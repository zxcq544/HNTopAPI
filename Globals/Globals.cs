using HNTopAPI.Models;
using System.Diagnostics;
using System.Text.Json;

namespace HNTopAPI.Globals
{
    public class Globals
    {
        public static List<Item> GlobalItems;
        public static List<Item> GlobalItemsSortedByScore;
        public static Dictionary<int, Dictionary<int, Item>> GlobalFullStories;
        public static async Task GetItems()
        {
            await GetTopStoriesFromHackerNews();
            //GlobalFullStories = GlobalComments
        }
        public static async Task GetTopStoriesFromHackerNews()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var Highscore = 200;
            var stopwatch = new Stopwatch();

            Console.WriteLine($"Starting http get request {DateTime.Now:dddd, dd MMMM yyyy HH:mm:ss}");
            HttpClient client = new();
            stopwatch.Start();
            string responseString = await client.GetStringAsync("https://hacker-news.firebaseio.com/v0/beststories.json");
            stopwatch.Stop();

            Console.WriteLine($"Response recieved in {(int)stopwatch.ElapsedMilliseconds} ms");
            int[] topStories = JsonSerializer.Deserialize<int[]>(responseString);

            //string oneItemString = await client.GetStringAsync($"https://hacker-news.firebaseio.com/v0/item/{topStories[1]}.json");

            //Console.WriteLine($"First item of array of ints {topStories[1]} num of top stories {topStories.Length}");
            //Console.WriteLine($"Full item {oneItemString}");

            //Item oneItem = JsonSerializer.Deserialize<Item>(oneItemString, options);
            //Console.WriteLine($"Full item deserialized");

            //foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(oneItem))
            //{
            //    string name = descriptor.Name;
            //    object value = descriptor.GetValue(oneItem);
            //    Console.WriteLine("{0} = {1}", name, value);
            //}
            Console.WriteLine($"Starting mass get reqests at {DateTime.Now:dddd, dd MMMM yyyy HH:mm:ss} for {topStories.Length} items");

            stopwatch.Restart();
            var tasks = topStories.Select(id => client.GetStringAsync($"https://hacker-news.firebaseio.com/v0/item/{id}.json"));
            string[] result = await Task.WhenAll(tasks);
            stopwatch.Stop();
            Console.WriteLine($"Completed mass reqeusts in {(int)stopwatch.ElapsedMilliseconds} ms");
            Console.WriteLine($"Count is {result.Length}");

            List<Item> allItems = (from onePage in result.AsParallel()
                                   let item = JsonSerializer.Deserialize<Item>(onePage, options)
                                   select item).ToList();
            List<Item> sortedItems = (from item in allItems.AsParallel()
                                      where (item.url != null) && (item.score >= Highscore)
                                      orderby item.DateTime.Date descending, item.score descending
                                      select item).ToList();
            var groupedItemsByDate = from item in sortedItems.AsParallel()
                                     orderby item.DateTime.Date descending
                                     group item by item.DateTime.Date into grp
                                     select new { Date = grp.Key, Count = grp.Count() };
            GlobalItemsSortedByScore = (from item in allItems.AsParallel()
                                        where (item.url != null) && (item.score >= Highscore)
                                        orderby item.score descending
                                        select item).ToList();
            GlobalItems = sortedItems;
            Dictionary<int, Item> sorted_items_as_dict = (from item in sortedItems.AsParallel()
                                                          select item).ToDictionary(id => id.id, item => item);

            foreach (var line in groupedItemsByDate)
            {
                Console.WriteLine($"Items count for {line.Date:dd.MM.yyyy} is {line.Count}");
            }
            Console.WriteLine($"Items above {Highscore} score count is {sortedItems.Count}");

            GlobalComments comments = new(sorted_items_as_dict, client, sortedItems);
            GlobalFullStories = await comments.get_all_comments_for_items();
            /*foreach (Item item in sortedItems)
            {
                Console.WriteLine($"Score={item.Score} Title={item.Title} Date={item.DateTime.Date:dd/MM/yyyy}");
            } */
        }

    }

}

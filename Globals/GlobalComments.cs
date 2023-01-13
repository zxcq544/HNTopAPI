using HNTopAPI.Models;
using Microsoft.Extensions.Configuration.CommandLine;
using System.Diagnostics;
using System.Text.Json;

namespace HNTopAPI.Globals
{
    public class GlobalComments
    {

        Dictionary<int, Item> sorted_items_as_dict;

        HttpClient client;

        List<Item> sortedItems;

        public GlobalComments(Dictionary<int, Item> sorted_items_as_dict, HttpClient client, List<Item> sortedItems)
        {
            this.sorted_items_as_dict = sorted_items_as_dict;
            this.client = client;
            this.sortedItems = sortedItems;
        }

        /* Returns dict which contains structure as follows:
*  {
*      //story id
*      3123123123:
*      {
*          // root comment - contains first level comments
*          -1:{
*              story stuff
*          },
*          //comment id
*          3123123123: {
*              comment stuff
*          },
*          ....many other comments
*      },
*      //story id 
*      123123123:
*      {
*          // root comment
*          -1:{
*              story stuff
*          },
*          //after root nodes come comments for story_id        
*          3123123123: {
*              comment stuff
*          }
*      }
*  
*  }
*/
        public async Task<Dictionary<int, Dictionary<int, Item>>> get_all_comments_for_items()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            Console.WriteLine("starting mass comments request");
            var tasks_for_comments = sortedItems.Select(item => get_comments_for_story_by_id(item.id));
            Dictionary<int, Item>[] full_stories_temp = await Task.WhenAll(tasks_for_comments);
            // -1 is root node which contains story id as id
            Dictionary<int, Dictionary<int, Item>> full_stories = (from item in full_stories_temp.AsParallel()
                                                                   select item).ToDictionary(key => key[-1].id, dict_id_item => dict_id_item);
            Console.WriteLine($"mass comments loaded in {stopwatch.Elapsed.TotalSeconds} sec");
            return full_stories;
        }

        async Task<Dictionary<int, Item>> get_comments_for_story_by_id(int id)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            Item story = sorted_items_as_dict[id];
            List<int> stack = new();
            List<Item> all_comments = new();
            stack.AddRange(story.kids);
            while (stack.Count > 0)
            {
                IEnumerable<Task<string>> tasks = stack.Select(comment_id => client.GetStringAsync($"https://hacker-news.firebaseio.com/v0/item/{comment_id}.json?print=pretty"));
                string[] results = await Task.WhenAll(tasks);
                List<Item> partial_comments = (from onePage in results.AsParallel()
                                               let item = JsonSerializer.Deserialize<Item>(onePage)
                                               select item).ToList();
                all_comments.AddRange(partial_comments);
                stack.Clear();
                foreach (Item comment in partial_comments)
                {
                    if (comment.kids != null)
                    {
                        stack.AddRange(comment.kids);
                    }
                }
            }
            var result = (from item in all_comments.AsParallel()
                          select new { item.id, item }).ToDictionary(dic_id_item => dic_id_item.id, dic_id_item => dic_id_item.item);
            result.Add(-1, story);
            Console.WriteLine($"loaded {result.Count} comments in {stopwatch.Elapsed.TotalMilliseconds} ms");
            count_children_for_each_comment(result);
            Console.WriteLine(result[-1]);
            return result;
        }

        void count_children_for_each_comment(Dictionary<int, Item> comments)
        {
            foreach (var comment in comments)
            {
                Item item = comments[comment.Key];
                if (item.kids != null)
                {
                    List<int> stack = new();
                    int num_kids = 0;
                    stack.AddRange(item.kids);
                    num_kids = item.kids.Length;
                    while (stack.Count > 0)
                    {
                        int current_id = stack[0];
                        stack.RemoveAt(0);
                        Item current_comment = comments[current_id];
                        if (current_comment.kids != null)
                        {
                            num_kids += current_comment.kids.Length;
                            stack.AddRange(current_comment.kids);
                        }
                        if (current_comment.deleted != null || current_comment.dead != null)
                        {
                            num_kids -= 1;
                        }
                    }
                    if (num_kids > 0)
                    {
                        item.num_kids = num_kids;
                        item.show_kids = true;
                    }
                }

            }

        }

    }
}

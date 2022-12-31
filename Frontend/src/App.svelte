<script>
    import { onMount } from "svelte";
    let items = [];
    onMount(async () => {
        getNewItems();
    });
    async function getBestItems() {
        const res = await fetch("/api/items?sort=best");
        items = await res.json();
    }
    async function getNewItems() {
        const res = await fetch("/api/items");
        items = await res.json();
    }
    function convertUnixTimeToDateTime(values) {
        values.forEach((value) => {
            value.DateTime = new Date(value.time * 1000);
        });
    }
    $: {
        convertUnixTimeToDateTime(items);
    }
</script>

<main>
    <h1>Hacker News Top</h1>
    <div class="button_block">
        <button on:click={getNewItems}>New</button>
        <button on:click={getBestItems}>Best</button>
    </div>
    <table class="table">
        <thead>
            <tr>
                <th> Score </th>
                <th> Title </th>
                <th> DateTime </th>
            </tr>
        </thead>
        <tbody>
            {#each items as item}
                <tr>
                    <td>
                        <h3>{item.score}</h3>
                    </td>
                    <td class="item_title">
                        <a href={item.url}>{item.title}</a>
                    </td>
                    <td>
                        {item.DateTime.getUTCDate()}.{item.DateTime.getUTCMonth() + 1}.{item.DateTime.getUTCFullYear()}
                    </td>
                </tr>
            {/each}
        </tbody>
    </table>
</main>

<style>
    .item_title {
        text-align: left;
    }
    .button_block {
        margin-bottom: 30px;
    }

    table {
        border-collapse: collapse;
        margin: 0 auto;
    }
    tr {
        border: solid;
        border-width: 1px 0;
    }
    tr:first-child {
        border-top: none;
    }
    tr:last-child {
        border-bottom: none;
    }
    main {
        width: 1000px;
    }
</style>

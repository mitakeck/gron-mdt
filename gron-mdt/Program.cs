using Markdig;
using Markdig.Extensions.Tables;

// string deco util

// abc => "abc"
// 1abc => "1abc"
// 1 => 1
// 1.2 => 1.2
// true => "true"
Dictionary<string, string> decoValueCache = new();
Func<string, string> decoValue = (string target) =>
{
    if (decoValueCache.ContainsKey(target))
    {
        return decoValueCache[target];
    }

    if (float.TryParse(target, out _))
    {
        decoValueCache.Add(target, target);
        return target;
    }

    var format = string.Format("\"{0}\"", target);
    decoValueCache.Add(target, format);
    return format;
};


Dictionary<string, string> decoKeyCache = new();
Func<string, string> decoKey = (string target) =>
{
    if (decoKeyCache.ContainsKey(target))
    {
        return decoKeyCache[target];
    }

    if (target.Contains(" ")) // TODO need to adapt js key spec
    {
        var fotmat = string.Format("[\"{0}\"]", target);
        decoKeyCache.Add(target, fotmat);
        return fotmat;
    }

    decoKeyCache.Add(target, target);
    return target;
};

Func<string, List<string>> parser = (string markdown) =>
{
    var pipeline = new MarkdownPipelineBuilder()
        //.UseAdvancedExtensions()
        .UsePipeTables()
        .Build();
    var mdast = Markdown.Parse(markdown, pipeline);

    List<string> result = new()
    {
        "json = {};",
    };
    List<string> headers = new();
    bool isHeader = true;
    int nameColIndex = 0;

    foreach (var element in mdast)
    {
        if (element.GetType() != typeof(Table))
        {
            continue;
        }

        Table table = (Table)element;

        foreach (TableRow row in table)
        {
            if (row.Count <= nameColIndex)
            {
                continue;
            }

            var nameSpan = row[nameColIndex].Span;
            var name = markdown.Substring(nameSpan.Start, nameSpan.Length).Trim();
            result.Add(string.Format("json.{0} = {1};", decoKey(name), "{}"));

            for (int i = 0; i < row.Count; i++)
            {
                var col = row[i];
                var text = markdown.Substring(col.Span.Start, col.Span.Length).Trim();

                if (isHeader)
                {
                    headers.Add(text);
                    continue;
                }
                //if (i == nameColIndex)
                //{
                //    continue;
                //}

                var header = headers[i];
                result.Add(string.Format("json.{0}.{1} = {2};", decoKey(name), decoKey(header), decoValue(text)));
            }
            isHeader = false;
        }

        break; // TODO multi tables in same md
    }

    return result;
};

var input = Console.In;
foreach (var item in parser(input.ReadToEnd()))
{
    Console.WriteLine(item);
}

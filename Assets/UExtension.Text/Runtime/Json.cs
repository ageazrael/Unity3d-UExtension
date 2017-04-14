using System.Collections;
using System.Collections.Generic;

namespace UExtension
{
    public enum JsonType
    {
        Class,
        Array,
        Data,
    }

    public abstract class JsonNode
    {
        public abstract JsonType            Type { get; }
    }
    public class JsonClass : JsonNode
    {
        public override JsonType            Type { get { return JsonType.Class; } }
        public Dictionary<string, JsonNode> Childs = new Dictionary<string, JsonNode>();


        public JsonNode this[string rName]
        {
            get { return this.Childs[rName]; }
            set { this.Childs[rName] = value; }
        }

        public void Add(string rName, JsonNode rNode)
        {
            this.Childs.Add(rName, rNode);
        }
        public void Remove(string rName)
        {
            this.Childs.Remove(rName);
        }
    }
    public class JsonArray : JsonNode
    {
        public override JsonType            Type { get { return JsonType.Array; } }
        public List<JsonNode>               Array = new List<JsonNode>();

        public JsonNode this[int nIndex]
        {
            get { return this.Array[nIndex]; }
            set { this.Array[nIndex] = value; }
        }

        public void Add(JsonNode rNode)
        {
            this.Array.Add(rNode);
        }
    }
    public class JsonData : JsonNode
    {
        public override JsonType            Type { get { return JsonType.Data; } }
        public string                       Data;
    }

    public class Json
    {
        public static JsonNode Parse(string text)
        {
            return Parse(new Tokenizer(text, true));
        }

        public static JsonNode Parse(Tokenizer rTokenizer)
        {
            JsonNode rNode = null;
            var rToken = rTokenizer.NextTokenAndSkipComment();
            if (rToken.Type == TokenType.Symbol)
            {
                if (rToken.Text == "{")
                {
                    var rJsonClass = new JsonClass();

                    rToken = rTokenizer.NextTokenAndSkipComment();
                    while (rToken.Text != "}")
                    {
                        var rJsonNodeName = string.Empty;
                        if (rToken.Type == TokenType.String ||
                            rToken.Type == TokenType.Identifie ||
                            rToken.Type == TokenType.Integer ||
                            rToken.Type == TokenType.Number)
                        {
                            rJsonNodeName = rToken.Text;
                        }

                        rToken = rTokenizer.NextTokenAndSkipComment();
                        /// if (rToken.Type == TokenType.Symbol && rToken.Text == ":")

                        var rJsonNode = Parse(rTokenizer);
                        if (null != rJsonNode)
                        {
                            rJsonClass.Add(rJsonNodeName, rJsonNode);
                        }

                        rToken = rTokenizer.NextToken();
                        if (rToken.Text == ",")
                            rToken = rTokenizer.NextTokenAndSkipComment();
                    }

                    return rJsonClass;
                }
                else if (rToken.Text == "[")
                {
                    var rJsonArray = new JsonArray();

                    rToken = rTokenizer.NextTokenAndSkipComment();
                    while (rToken.Text != "]")
                    {
                        var rJsonNode = Parse(rTokenizer);
                        if (null != rJsonNode)
                            rJsonArray.Add(rJsonNode);
                    }

                    return rJsonArray;
                }
            }
            else
            {
                rNode = new JsonData() {
                    Data = rToken.Text
                };
            }

            return rNode;
        }
    }

}
using UnityEngine;
using UnityEditor;
using NUnit.Framework;

using UExtension;

namespace UnitTest.Common
{
    public class TokenizerUnitTest
    {
        [Test]
        public void NextToken()
        {
            {
                var rText = "abs(\"Expand\", 56, 20.3, .5f)";
                var rTokenizer = new Tokenizer(rText, false);

                DoTestToken(rTokenizer.NextToken(), "abs",      TokenType.Identifie);
                DoTestToken(rTokenizer.NextToken(), "(",        TokenType.Symbol);
                DoTestToken(rTokenizer.NextToken(), "Expand",   TokenType.String);
                DoTestToken(rTokenizer.NextToken(), ",",        TokenType.Symbol);
                DoTestToken(rTokenizer.NextToken(), "56",       TokenType.Integer);
                DoTestToken(rTokenizer.NextToken(), ",",        TokenType.Symbol);
                DoTestToken(rTokenizer.NextToken(), "20.3",     TokenType.Number);
                DoTestToken(rTokenizer.NextToken(), ",",        TokenType.Symbol);
                DoTestToken(rTokenizer.NextToken(), ".5",       TokenType.Number);
                DoTestToken(rTokenizer.NextToken(), "f",        TokenType.Identifie);
                DoTestToken(rTokenizer.NextToken(), ")",        TokenType.Symbol);
            }
        }

        public void DoTestToken(Token rToken, string rText, TokenType rTokenType)
        {
            Assert.True(rToken.Valid);
            Assert.AreEqual(rToken.Text, rText);
            Assert.AreEqual(rToken.Type, rTokenType);
        }
    }
}
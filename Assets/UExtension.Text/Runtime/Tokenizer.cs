using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UExtension
{
    public enum TokenType
    {
        Unknown,
        Integer,        // 0000
        Number,         // 000.0000
        String,         // \" - \"
        Symbol,         // 
        Identifie,      // A-Z | a-z | _ | 0-9

        LineComment,    // //.....
        RangeComment,   // /*...*/
    }
    public class Token
    {
        public const int InvalidIndex = -1;
        /// <summary>
        /// Start - End   abs("Param");
        ///     Identifie(abs)      => StartIndex=0,EndIndex=2
        ///     Symbol('(')         => StartIndex=4,EndIndex=4
        ///     String(Param)       => StartIndex=5,EndIndex=9
        ///     Symbol(')')         => StartIndex=11,EndIndex=11
        /// </summary>
        public TokenType    Type        = TokenType.Unknown;
        public int          StartIndex  = InvalidIndex;
        public int          EndIndex    = InvalidIndex;
        public string       SourceText;

        public string Text
        {
            get { return Valid ? SourceText.Substring(StartIndex, EndIndex - StartIndex + 1) : string.Empty; }
        }
        public bool Valid
        {
            get
            {
                return !string.IsNullOrEmpty(SourceText) &&
                    (StartIndex >= 0 && StartIndex < SourceText.Length) &&
                    (EndIndex >= 0 && EndIndex < SourceText.Length);
            }
        }
    }

    public class Tokenizer
    {
        public const string SymbolText = "`~!@#$%^&*()-+=[]{}\\|;:'<>,./?";

        public Tokenizer(string rSourceText, bool bAllowComment)
        {
            mSourceText         = rSourceText;
            mAllowComment       = bAllowComment;
            mCurrentSourceIndex = 0;
        }

        public void Restart()
        {
            mCurrentSourceIndex = 0;
        }

        public Token NextTokenAndSkipComment()
        {
            var rToken = this.NextToken();
            while (rToken.Type == TokenType.LineComment || rToken.Type == TokenType.RangeComment)
                rToken = this.NextToken();

            return rToken;
        }

        public Token NextToken()
        {
            var rToken = new Token();

            // Init
            rToken.Type         = TokenType.Unknown;
            rToken.SourceText   = mSourceText;
            rToken.EndIndex     = mSourceText.Length - 1;

            var bTokening = this.IsValid();
            while (bTokening)
            {
                var rChar = this.CurrentChar();
                if (rToken.Type == TokenType.Unknown)
                {
                    if (mAllowComment && rChar == '/' && this.NextChar() == '/')
                    {
                        rToken.Type = TokenType.LineComment;
                        rToken.StartIndex = mCurrentSourceIndex;
                        this.Next();
                        this.Next();
                    }
                    else if (mAllowComment && rChar == '/' && this.NextChar() == '*')
                    {
                        rToken.Type = TokenType.LineComment;
                        rToken.StartIndex = mCurrentSourceIndex;
                        this.Next();
                        this.Next();
                    }
                    else if (rChar == '.' && this.NextChar().IsDigit())
                    {
                        rToken.Type         = TokenType.Number;
                        rToken.StartIndex   = mCurrentSourceIndex;
                        this.Next();
                    }
                    else if (SymbolText.IndexOf(rChar) != -1)
                    {
                        this.ApplySymbol(rToken);
                        this.Next();
                        bTokening = false;
                        break;
                    }
                    else if (rChar.IsAlpha() || rChar == '_')
                    {
                        rToken.Type = TokenType.Identifie;
                        rToken.StartIndex = mCurrentSourceIndex;
                        this.Next();
                    }
                    else if (rChar == '"')
                    {
                        rToken.Type = this.CanNext() ? TokenType.String : TokenType.Unknown;
                        rToken.StartIndex = this.CanNext() ? mCurrentSourceIndex + 1 : Token.InvalidIndex;
                        this.Next();
                    }
                    else if (rChar.IsDigit())
                    {
                        rToken.Type = TokenType.Integer;
                        rToken.StartIndex = mCurrentSourceIndex;
                        this.Next();
                    }
                    else
                    {
                        this.Next();
                    }
                }
                else if (rToken.Type == TokenType.String)
                {
                    if (rChar == '"')
                    {
                        rToken.EndIndex = mCurrentSourceIndex - 1;
                        bTokening = false;
                        this.Next();
                        break;
                    }
                    else
                    {
                        this.Next();
                    }
                }
                else if (rToken.Type == TokenType.Identifie)
                {
                    if ((!rChar.IsAlpha() && !rChar.IsDigit() && rChar != '_') ||
                        (rChar == '/' && this.NextChar() == '/') ||
                        (rChar == '/' && this.NextChar() == '*'))
                    {
                        rToken.EndIndex = mCurrentSourceIndex - 1;
                        bTokening = false;
                        break;
                    }
                    else
                    {
                        this.Next();
                    }
                }
                else if (rToken.Type == TokenType.LineComment)
                {
                    if (rChar == '\n')
                    {
                        rToken.EndIndex = mCurrentSourceIndex - 1;
                        bTokening = false;
                        this.Next();
                        break;
                    }
                    else
                    {
                        this.Next();
                    }
                }
                else if (rToken.Type == TokenType.RangeComment)
                {
                    if (rChar == '*' && this.NextChar() == '/')
                    {
                        rToken.EndIndex = mCurrentSourceIndex - 1;
                        bTokening = false;
                        this.Next();
                        this.Next();
                        break;
                    }
                    else
                    {
                        
                        this.Next();
                    }
                }
                else if (rToken.Type == TokenType.Integer)
                {
                    if (rChar.IsDigit())
                    {
                        this.Next();
                    }
                    else if (rChar == '.' && this.NextChar().IsDigit())
                    {
                        rToken.Type = TokenType.Number;
                        this.Next();
                    }
                    else
                    {
                        rToken.EndIndex = mCurrentSourceIndex - 1;
                        bTokening = false;
                        break;
                    }
                }
                else if (rToken.Type == TokenType.Number)
                {
                    if (rChar.IsDigit())
                    {
                        this.Next();
                    }
                    else
                    {
                        rToken.EndIndex = mCurrentSourceIndex - 1;
                        bTokening = false;
                        break;
                    }
                }
                else
                {
                    this.Next();
                }

                if (!this.IsValid())
                    bTokening = false;
            }

            return rToken;
        }

        // { State Controller
        public bool IsValid()
        {
            return !string.IsNullOrEmpty(mSourceText) &&
                mCurrentSourceIndex >= 0 && mCurrentSourceIndex < mSourceText.Length;
        }
        protected bool CanNext()
        {
            return !string.IsNullOrEmpty(mSourceText) &&
                (mCurrentSourceIndex + 1) >= 0 && (mCurrentSourceIndex + 1) < mSourceText.Length;
        }
        protected void Next()
        {
            if (IsValid())
                mCurrentSourceIndex++;
        }
        protected void Prev()
        {
            if (mCurrentSourceIndex > 0)
                mCurrentSourceIndex--;
        }
        protected char PrevChar()
        {
            return (IsValid() && mCurrentSourceIndex > 0) ? mSourceText[mCurrentSourceIndex - 1] : (char)0;
        }
        protected char CurrentChar()
        {
            return IsValid() ? mSourceText[mCurrentSourceIndex] : (char)0;
        }
        protected char NextChar()
        {
            return CanNext() ? mSourceText[mCurrentSourceIndex + 1] : (char)0;
        }
        // } Start Controller

        protected void ApplySymbol(Token rToken)
        {
            if (!IsValid())
                return;

            rToken.Type         = TokenType.Symbol;
            rToken.StartIndex   = mCurrentSourceIndex;
            rToken.EndIndex     = mCurrentSourceIndex;
        }


        protected string    mSourceText;
        protected bool      mAllowComment;
        protected int       mCurrentSourceIndex;
        protected string    mSymbolText;
    }
}

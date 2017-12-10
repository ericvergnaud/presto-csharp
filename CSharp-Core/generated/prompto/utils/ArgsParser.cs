//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.7.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from ArgsParser.g4 by ANTLR 4.7.1

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

namespace prompto.utils {
using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using DFA = Antlr4.Runtime.Dfa.DFA;

[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.7.1")]
[System.CLSCompliant(false)]
public partial class ArgsParser : Parser {
	protected static DFA[] decisionToDFA;
	protected static PredictionContextCache sharedContextCache = new PredictionContextCache();
	public const int
		STRING=1, EQUALS=2, DASH=3, WS=4, ELEMENT=5;
	public const int
		RULE_parse = 0, RULE_entry = 1, RULE_key = 2, RULE_value = 3;
	public static readonly string[] ruleNames = {
		"parse", "entry", "key", "value"
	};

	private static readonly string[] _LiteralNames = {
		null, null, "'='", "'-'", "' '"
	};
	private static readonly string[] _SymbolicNames = {
		null, "STRING", "EQUALS", "DASH", "WS", "ELEMENT"
	};
	public static readonly IVocabulary DefaultVocabulary = new Vocabulary(_LiteralNames, _SymbolicNames);

	[NotNull]
	public override IVocabulary Vocabulary
	{
		get
		{
			return DefaultVocabulary;
		}
	}

	public override string GrammarFileName { get { return "ArgsParser.g4"; } }

	public override string[] RuleNames { get { return ruleNames; } }

	public override string SerializedAtn { get { return new string(_serializedATN); } }

	static ArgsParser() {
		decisionToDFA = new DFA[_ATN.NumberOfDecisions];
		for (int i = 0; i < _ATN.NumberOfDecisions; i++) {
			decisionToDFA[i] = new DFA(_ATN.GetDecisionState(i), i);
		}
	}

		public ArgsParser(ITokenStream input) : this(input, Console.Out, Console.Error) { }

		public ArgsParser(ITokenStream input, TextWriter output, TextWriter errorOutput)
		: base(input, output, errorOutput)
	{
		Interpreter = new ParserATNSimulator(this, _ATN, decisionToDFA, sharedContextCache);
	}
	public partial class ParseContext : ParserRuleContext {
		public EntryContext e;
		public EntryContext[] entry() {
			return GetRuleContexts<EntryContext>();
		}
		public EntryContext entry(int i) {
			return GetRuleContext<EntryContext>(i);
		}
		public ParseContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_parse; } }
		public override void EnterRule(IParseTreeListener listener) {
			IArgsParserListener typedListener = listener as IArgsParserListener;
			if (typedListener != null) typedListener.EnterParse(this);
		}
		public override void ExitRule(IParseTreeListener listener) {
			IArgsParserListener typedListener = listener as IArgsParserListener;
			if (typedListener != null) typedListener.ExitParse(this);
		}
	}

	[RuleVersion(0)]
	public ParseContext parse() {
		ParseContext _localctx = new ParseContext(Context, State);
		EnterRule(_localctx, 0, RULE_parse);
		int _la;
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 11;
			ErrorHandler.Sync(this);
			_la = TokenStream.LA(1);
			while (_la==DASH || _la==ELEMENT) {
				{
				{
				State = 8; _localctx.e = entry();
				}
				}
				State = 13;
				ErrorHandler.Sync(this);
				_la = TokenStream.LA(1);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class EntryContext : ParserRuleContext {
		public KeyContext k;
		public ValueContext v;
		public ITerminalNode EQUALS() { return GetToken(ArgsParser.EQUALS, 0); }
		public KeyContext key() {
			return GetRuleContext<KeyContext>(0);
		}
		public ValueContext value() {
			return GetRuleContext<ValueContext>(0);
		}
		public ITerminalNode DASH() { return GetToken(ArgsParser.DASH, 0); }
		public EntryContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_entry; } }
		public override void EnterRule(IParseTreeListener listener) {
			IArgsParserListener typedListener = listener as IArgsParserListener;
			if (typedListener != null) typedListener.EnterEntry(this);
		}
		public override void ExitRule(IParseTreeListener listener) {
			IArgsParserListener typedListener = listener as IArgsParserListener;
			if (typedListener != null) typedListener.ExitEntry(this);
		}
	}

	[RuleVersion(0)]
	public EntryContext entry() {
		EntryContext _localctx = new EntryContext(Context, State);
		EnterRule(_localctx, 2, RULE_entry);
		int _la;
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 15;
			ErrorHandler.Sync(this);
			_la = TokenStream.LA(1);
			if (_la==DASH) {
				{
				State = 14; Match(DASH);
				}
			}

			State = 17; _localctx.k = key();
			State = 18; Match(EQUALS);
			State = 19; _localctx.v = value();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class KeyContext : ParserRuleContext {
		public ITerminalNode ELEMENT() { return GetToken(ArgsParser.ELEMENT, 0); }
		public KeyContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_key; } }
		public override void EnterRule(IParseTreeListener listener) {
			IArgsParserListener typedListener = listener as IArgsParserListener;
			if (typedListener != null) typedListener.EnterKey(this);
		}
		public override void ExitRule(IParseTreeListener listener) {
			IArgsParserListener typedListener = listener as IArgsParserListener;
			if (typedListener != null) typedListener.ExitKey(this);
		}
	}

	[RuleVersion(0)]
	public KeyContext key() {
		KeyContext _localctx = new KeyContext(Context, State);
		EnterRule(_localctx, 4, RULE_key);
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 21; Match(ELEMENT);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class ValueContext : ParserRuleContext {
		public ValueContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_value; } }
	 
		public ValueContext() { }
		public virtual void CopyFrom(ValueContext context) {
			base.CopyFrom(context);
		}
	}
	public partial class ELEMENTContext : ValueContext {
		public ITerminalNode ELEMENT() { return GetToken(ArgsParser.ELEMENT, 0); }
		public ELEMENTContext(ValueContext context) { CopyFrom(context); }
		public override void EnterRule(IParseTreeListener listener) {
			IArgsParserListener typedListener = listener as IArgsParserListener;
			if (typedListener != null) typedListener.EnterELEMENT(this);
		}
		public override void ExitRule(IParseTreeListener listener) {
			IArgsParserListener typedListener = listener as IArgsParserListener;
			if (typedListener != null) typedListener.ExitELEMENT(this);
		}
	}
	public partial class STRINGContext : ValueContext {
		public ITerminalNode STRING() { return GetToken(ArgsParser.STRING, 0); }
		public STRINGContext(ValueContext context) { CopyFrom(context); }
		public override void EnterRule(IParseTreeListener listener) {
			IArgsParserListener typedListener = listener as IArgsParserListener;
			if (typedListener != null) typedListener.EnterSTRING(this);
		}
		public override void ExitRule(IParseTreeListener listener) {
			IArgsParserListener typedListener = listener as IArgsParserListener;
			if (typedListener != null) typedListener.ExitSTRING(this);
		}
	}

	[RuleVersion(0)]
	public ValueContext value() {
		ValueContext _localctx = new ValueContext(Context, State);
		EnterRule(_localctx, 6, RULE_value);
		try {
			State = 25;
			ErrorHandler.Sync(this);
			switch (TokenStream.LA(1)) {
			case ELEMENT:
				_localctx = new ELEMENTContext(_localctx);
				EnterOuterAlt(_localctx, 1);
				{
				State = 23; Match(ELEMENT);
				}
				break;
			case STRING:
				_localctx = new STRINGContext(_localctx);
				EnterOuterAlt(_localctx, 2);
				{
				State = 24; Match(STRING);
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	private static char[] _serializedATN = {
		'\x3', '\x608B', '\xA72A', '\x8133', '\xB9ED', '\x417C', '\x3BE7', '\x7786', 
		'\x5964', '\x3', '\a', '\x1E', '\x4', '\x2', '\t', '\x2', '\x4', '\x3', 
		'\t', '\x3', '\x4', '\x4', '\t', '\x4', '\x4', '\x5', '\t', '\x5', '\x3', 
		'\x2', '\a', '\x2', '\f', '\n', '\x2', '\f', '\x2', '\xE', '\x2', '\xF', 
		'\v', '\x2', '\x3', '\x3', '\x5', '\x3', '\x12', '\n', '\x3', '\x3', '\x3', 
		'\x3', '\x3', '\x3', '\x3', '\x3', '\x3', '\x3', '\x4', '\x3', '\x4', 
		'\x3', '\x5', '\x3', '\x5', '\x5', '\x5', '\x1C', '\n', '\x5', '\x3', 
		'\x5', '\x2', '\x2', '\x6', '\x2', '\x4', '\x6', '\b', '\x2', '\x2', '\x2', 
		'\x1C', '\x2', '\r', '\x3', '\x2', '\x2', '\x2', '\x4', '\x11', '\x3', 
		'\x2', '\x2', '\x2', '\x6', '\x17', '\x3', '\x2', '\x2', '\x2', '\b', 
		'\x1B', '\x3', '\x2', '\x2', '\x2', '\n', '\f', '\x5', '\x4', '\x3', '\x2', 
		'\v', '\n', '\x3', '\x2', '\x2', '\x2', '\f', '\xF', '\x3', '\x2', '\x2', 
		'\x2', '\r', '\v', '\x3', '\x2', '\x2', '\x2', '\r', '\xE', '\x3', '\x2', 
		'\x2', '\x2', '\xE', '\x3', '\x3', '\x2', '\x2', '\x2', '\xF', '\r', '\x3', 
		'\x2', '\x2', '\x2', '\x10', '\x12', '\a', '\x5', '\x2', '\x2', '\x11', 
		'\x10', '\x3', '\x2', '\x2', '\x2', '\x11', '\x12', '\x3', '\x2', '\x2', 
		'\x2', '\x12', '\x13', '\x3', '\x2', '\x2', '\x2', '\x13', '\x14', '\x5', 
		'\x6', '\x4', '\x2', '\x14', '\x15', '\a', '\x4', '\x2', '\x2', '\x15', 
		'\x16', '\x5', '\b', '\x5', '\x2', '\x16', '\x5', '\x3', '\x2', '\x2', 
		'\x2', '\x17', '\x18', '\a', '\a', '\x2', '\x2', '\x18', '\a', '\x3', 
		'\x2', '\x2', '\x2', '\x19', '\x1C', '\a', '\a', '\x2', '\x2', '\x1A', 
		'\x1C', '\a', '\x3', '\x2', '\x2', '\x1B', '\x19', '\x3', '\x2', '\x2', 
		'\x2', '\x1B', '\x1A', '\x3', '\x2', '\x2', '\x2', '\x1C', '\t', '\x3', 
		'\x2', '\x2', '\x2', '\x5', '\r', '\x11', '\x1B',
	};

	public static readonly ATN _ATN =
		new ATNDeserializer().Deserialize(_serializedATN);


}
} // namespace prompto.utils

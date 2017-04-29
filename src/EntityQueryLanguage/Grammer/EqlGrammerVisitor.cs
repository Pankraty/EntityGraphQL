//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.6.1-SNAPSHOT
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from src/EntityQueryLanguage/Grammer/EqlGrammer.g4 by ANTLR 4.6.1-SNAPSHOT

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

namespace EntityQueryLanguage.Grammer {
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using IToken = Antlr4.Runtime.IToken;

/// <summary>
/// This interface defines a complete generic visitor for a parse tree produced
/// by <see cref="EqlGrammerParser"/>.
/// </summary>
/// <typeparam name="Result">The return type of the visit operation.</typeparam>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.6.1-SNAPSHOT")]
public interface IEqlGrammerVisitor<Result> : IParseTreeVisitor<Result> {
	/// <summary>
	/// Visit a parse tree produced by the <c>const</c>
	/// labeled alternative in <see cref="EqlGrammerParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitConst([NotNull] EqlGrammerParser.ConstContext context);

	/// <summary>
	/// Visit a parse tree produced by the <c>ifThenElse</c>
	/// labeled alternative in <see cref="EqlGrammerParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitIfThenElse([NotNull] EqlGrammerParser.IfThenElseContext context);

	/// <summary>
	/// Visit a parse tree produced by the <c>binary</c>
	/// labeled alternative in <see cref="EqlGrammerParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitBinary([NotNull] EqlGrammerParser.BinaryContext context);

	/// <summary>
	/// Visit a parse tree produced by the <c>ifThenElseInline</c>
	/// labeled alternative in <see cref="EqlGrammerParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitIfThenElseInline([NotNull] EqlGrammerParser.IfThenElseInlineContext context);

	/// <summary>
	/// Visit a parse tree produced by the <c>expr</c>
	/// labeled alternative in <see cref="EqlGrammerParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitExpr([NotNull] EqlGrammerParser.ExprContext context);

	/// <summary>
	/// Visit a parse tree produced by the <c>callOrId</c>
	/// labeled alternative in <see cref="EqlGrammerParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitCallOrId([NotNull] EqlGrammerParser.CallOrIdContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="EqlGrammerParser.identity"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitIdentity([NotNull] EqlGrammerParser.IdentityContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="EqlGrammerParser.callPath"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitCallPath([NotNull] EqlGrammerParser.CallPathContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="EqlGrammerParser.int"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitInt([NotNull] EqlGrammerParser.IntContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="EqlGrammerParser.decimal"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitDecimal([NotNull] EqlGrammerParser.DecimalContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="EqlGrammerParser.string"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitString([NotNull] EqlGrammerParser.StringContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="EqlGrammerParser.constant"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitConstant([NotNull] EqlGrammerParser.ConstantContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="EqlGrammerParser.call"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitCall([NotNull] EqlGrammerParser.CallContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="EqlGrammerParser.args"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitArgs([NotNull] EqlGrammerParser.ArgsContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="EqlGrammerParser.operator"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitOperator([NotNull] EqlGrammerParser.OperatorContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="EqlGrammerParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitExpression([NotNull] EqlGrammerParser.ExpressionContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="EqlGrammerParser.startRule"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitStartRule([NotNull] EqlGrammerParser.StartRuleContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="EqlGrammerParser.ws"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitWs([NotNull] EqlGrammerParser.WsContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="EqlGrammerParser.field"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitField([NotNull] EqlGrammerParser.FieldContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="EqlGrammerParser.aliasType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitAliasType([NotNull] EqlGrammerParser.AliasTypeContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="EqlGrammerParser.aliasExp"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitAliasExp([NotNull] EqlGrammerParser.AliasExpContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="EqlGrammerParser.fieldSelect"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitFieldSelect([NotNull] EqlGrammerParser.FieldSelectContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="EqlGrammerParser.entityQuery"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitEntityQuery([NotNull] EqlGrammerParser.EntityQueryContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="EqlGrammerParser.dataQuery"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitDataQuery([NotNull] EqlGrammerParser.DataQueryContext context);
}
} // namespace EntityQueryLanguage.Grammer

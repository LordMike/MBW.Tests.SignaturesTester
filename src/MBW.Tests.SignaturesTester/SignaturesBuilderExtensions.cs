using System;
using System.Linq;

namespace MBW.Tests.SignaturesTester;

public static class SignaturesBuilderExtensions
{
    public static SignaturesBuilder MustHave(this SignaturesBuilder builder, string signature, params SignaturesBuilder.ArgumentEvaluator[] argT) => builder.MustHave(signature, argT.Select(s => (SignaturesBuilder.ArgumentEvaluatorMultiple)(x => new[] { s(x) })).ToArray());
    public static SignaturesBuilder MustHave<T>(this SignaturesBuilder builder, string signature) => builder.MustHave(signature, _ => typeof(T));
    public static SignaturesBuilder MustHave<T1, T2>(this SignaturesBuilder builder, string signature) => builder.MustHave(signature, _ => typeof(T1), _ => typeof(T2));

    public static SignaturesBuilder MustHaveProperty<TProperty>(this SignaturesBuilder builder, string name, bool hasGet = true, bool hasSet = true, bool isInstance = true) => builder.MustHaveProperty(name, typeof(TProperty), hasGet, hasSet, isInstance);
    public static SignaturesBuilder MustHaveProperty(this SignaturesBuilder builder, string name, Type propertyType, bool hasGet = true, bool hasSet = true, bool isInstance = true) => builder.MustHaveProperty(name, _ => propertyType, hasGet, hasSet, isInstance);
    public static SignaturesBuilder MustHaveProperty(this SignaturesBuilder builder, string name, SignaturesBuilder.ArgumentEvaluator propertyType, bool hasGet = true, bool hasSet = true, bool isInstance = true)
    {
        string modifier = isInstance ? "instance" : "static";

        if (hasGet)
            builder.MustHave($"{modifier} T1 get_{name}()", propertyType);
        if (hasSet)
            builder.MustHave($"{modifier} Void set_{name}(T1)", propertyType);

        return builder;
    }
}

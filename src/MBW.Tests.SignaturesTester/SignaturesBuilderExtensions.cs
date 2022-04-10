using System;
using System.Linq;

namespace MBW.Tests.SignaturesTester;

public static class SignaturesBuilderExtensions
{
    public static SignaturesBuilder MustHave(this SignaturesBuilder builder, string signature, params SignaturesBuilder.ArgumentEvaluator[] argT) => builder.MustHave(signature, type => argT.Select(s => s(type)).ToArray());
    public static SignaturesBuilder MustHave<T1>(this SignaturesBuilder builder, string signature) => builder.MustHave(signature, typeof(T1));
    public static SignaturesBuilder MustHave<T1, T2>(this SignaturesBuilder builder, string signature) => builder.MustHave(signature, typeof(T1), typeof(T2));
    public static SignaturesBuilder MustHave<T1, T2, T3>(this SignaturesBuilder builder, string signature) => builder.MustHave(signature, typeof(T1), typeof(T2), typeof(T3));
    public static SignaturesBuilder MustHave<T1, T2, T3, T4>(this SignaturesBuilder builder, string signature) => builder.MustHave(signature, typeof(T1), typeof(T2), typeof(T3), typeof(T4));
    public static SignaturesBuilder MustHave(this SignaturesBuilder builder, string signature, params Type[] arguments) => builder.MustHave(signature, _ => arguments);

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

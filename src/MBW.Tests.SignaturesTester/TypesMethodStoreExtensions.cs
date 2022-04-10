using System;
using System.Linq;

namespace MBW.Tests.SignaturesTester;

public static class TypesMethodStoreExtensions
{
    public static TypesMethodStore Add<T>(this TypesMethodStore store) => store.Add(typeof(T));
    public static TypesMethodStore Add(this TypesMethodStore store, params Type[] types) => types.Aggregate(store, (a, b) => a.Add(b));
}

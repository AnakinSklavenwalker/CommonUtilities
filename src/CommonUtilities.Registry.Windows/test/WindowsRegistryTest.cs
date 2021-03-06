using System;
using Microsoft.Win32;
using Xunit;

namespace Sklavenwalker.CommonUtilities.Registry.Windows.Test;

public class WindowsRegistryKeyTest : IDisposable
{
    private const string SubKey = @"SOFTWARE\CommonUtilities.Registry.Windows.Test";
    private readonly IRegistryKey _registryKey;

    private readonly RegistryKey w_registryKey;

    public WindowsRegistryKeyTest()
    {
#if NET
        if (!OperatingSystem.IsWindows())
            return;
#endif
        var baseKey = new WindowsRegistry().OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Default);
        _registryKey = baseKey.CreateSubKey(SubKey)!;
        w_registryKey = RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.CurrentUser, Microsoft.Win32.RegistryView.Default)
            .OpenSubKey(SubKey)!;
    }

    [Fact]
    public void TestExists()
    {
#if NET
        if (!OperatingSystem.IsWindows())
            return;
#endif
        Assert.True(_registryKey.HasPath(""));
        Assert.NotNull(w_registryKey);
    }

    [Fact]
    public void TestCrud()
    {
#if NET
        if (!OperatingSystem.IsWindows())
            return;
#endif
        _registryKey.SetValue("Test", true);
        Assert.NotNull(w_registryKey.GetValue("Test"));

        _registryKey.DeleteValue("Test");
        Assert.Null(w_registryKey.GetValue("Test"));

        _registryKey.CreateSubKey("Sub\\SubSub");
        var deleted = _registryKey.DeleteKey("Sub", false);
        Assert.False(deleted);
        Assert.NotNull(w_registryKey.OpenSubKey("Sub\\SubSub"));

        deleted = _registryKey.DeleteKey("Sub", true);
        Assert.True(deleted);
        Assert.Null(w_registryKey.OpenSubKey("Sub\\SubSub"));

    }

    [Fact]
    public void TestCrudSub()
    {
#if NET
        if (!OperatingSystem.IsWindows())
            return;
#endif
        var sub = _registryKey.CreateSubKey("Sub")!;
        var w_sub = w_registryKey.OpenSubKey("Sub");
        Assert.NotNull(w_sub);

        sub.SetValue("Test", true);
        Assert.NotNull(w_sub.GetValue("Test"));

        _registryKey.DeleteValue("Test", "Sub");
        Assert.Null(w_sub.GetValue("Test"));

        var deleted = _registryKey.DeleteKey("Sub", false);
        Assert.True(deleted);
        Assert.Null(w_registryKey.OpenSubKey("Sub"));
    }

    [Fact]
    public void TestSelfDelete()
    {
#if NET
        if (!OperatingSystem.IsWindows())
            return;
#endif
        _registryKey.DeleteValue("Test");
        Assert.Null(w_registryKey.GetValue("Test"));

        var sub = _registryKey.CreateSubKey("Sub");
        var deleted = sub!.DeleteKey("", true);
        Assert.True(deleted);
        Assert.Null(w_registryKey.OpenSubKey("Sub"));
    }

    [Fact]
    public void TestDataTypes()
    {
#if NET
        if (!OperatingSystem.IsWindows())
            return;
#endif
        _registryKey.SetValue("TestEnum", 1);
        _registryKey.GetValue("TestEnum", out int oi);
        var i = Assert.IsType<int>(oi);
        Assert.Equal(1, i);

        _registryKey.SetValue("TestEnum", 1ul);
        _registryKey.GetValue("TestEnum", out ulong oul);
        var ul = Assert.IsType<ulong>(oul);
        Assert.Equal(1ul, ul);

        _registryKey.SetValue("TestBool", true);
        _registryKey.GetValue("TestBool", out bool ob);
        var b = Assert.IsType<bool>(ob);
        Assert.True(b);

        _registryKey.SetValue("TestEnum", MyEnum.B);
        _registryKey.GetValue("TestEnum", out MyEnum oe);
        var e = Assert.IsType<MyEnum>(oe);
        Assert.Equal(MyEnum.B, e);
    }

    public void Dispose()
    {
#if NET
        if (!OperatingSystem.IsWindows())
            return;
#endif
        var wBase = RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.CurrentUser, Microsoft.Win32.RegistryView.Default);
        wBase.DeleteSubKeyTree(SubKey);
        _registryKey.Dispose();
    }

    private enum MyEnum
    {
        A,
        B
    }
}
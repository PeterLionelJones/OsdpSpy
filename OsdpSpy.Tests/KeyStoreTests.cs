using System.Linq;
using McMaster.Extensions.CommandLineUtils;
using Moq;
using NUnit.Framework;

namespace OsdpSpy.Tests;

[TestFixture]
public class KeyStoreTests
{
    private readonly byte[] _uid1 =  { 0x87, 0x11, 0xCD, 0x07, 0xD3, 0xD3, 0xF5, 0xAA, 0x93, 0x4D };
    private readonly byte[] _uid2 =  { 0xC2, 0xDD, 0x55, 0xDE, 0x2D, 0x08, 0x59, 0x30, 0x72, 0x9D };
    private readonly byte[] _uid3 =  { 0xA4, 0x06, 0xC5, 0x5A, 0x8D, 0xE0, 0xE4, 0x5A, 0x8A, 0xAF };
    private readonly byte[] _uid4 =  { 0xEE, 0xCC, 0x06, 0x5F, 0x72, 0xD9, 0x37, 0xE2, 0x15, 0x7D };

    private readonly byte[] _key1 =
    {
        0x2C, 0x07, 0x07, 0xE0, 0x41, 0x43, 0xFA, 0xF6, 
        0x51, 0x85, 0x5D, 0xC0, 0x17, 0x51, 0xBB, 0x89
    };

    private readonly byte[] _key2 =
    {
        0x87, 0x1C, 0x82, 0x9A, 0xC3, 0xAC, 0x09, 0xFA, 
        0x33, 0xE6, 0xEF, 0x08, 0x36, 0x99, 0x0A, 0x21
    };

    private readonly byte[] _key3 =
    {
        0x45, 0xA2, 0x3D, 0x33, 0xA3, 0x82, 0x8B, 0xDC, 
        0x3F, 0x43, 0xF2, 0xCD, 0xCF, 0xBB, 0x1E, 0x80
    };

    private readonly byte[] _key4 =
    {
        0x7E, 0x43, 0x8F, 0x59, 0xFF, 0x13, 0x25, 0xD6, 
        0xAE, 0x5B, 0x52, 0x50, 0x40, 0xBB, 0x41, 0x4A
    };

    [Test]
    public void Clear_ClearList_ListIsClear()
    {
        var console = new Mock<IConsole>();
        
        var testObject = new KeyStore(console.Object);
        testObject.Clear();

        var testObject2 = new KeyStore(console.Object);
        
        Assert.IsNotNull(testObject.KeyItemList);
        Assert.That(testObject.KeyItemList.Count == 0);
        Assert.IsNotNull(testObject2.KeyItemList);
        Assert.That(testObject2.KeyItemList.Count == 0);
    }

    [Test]
    public void Store_StoreKeyList_KeyListStoredCorrectly()
    {
        var console = new Mock<IConsole>();
        
        var testObject = new KeyStore(console.Object);
        testObject.Clear();
        testObject.Store(_uid1, _key1);        
        testObject.Store(_uid2, _key2);        
        testObject.Store(_uid3, _key3);        
        testObject.Store(_uid4, _key4);        
        
        Assert.IsNotNull(testObject.KeyItemList);
        Assert.That(testObject.KeyItemList.Count == 4);
    }

    [Test]
    public void Store_StoreSameTwice_KeyListStoredCorrectly()
    {
        var console = new Mock<IConsole>();
        
        var testObject = new KeyStore(console.Object);
        testObject.Clear();
        testObject.Store(_uid1, _key1);        
        testObject.Store(_uid1, _key1);        
        
        Assert.IsNotNull(testObject.KeyItemList);
        Assert.That(testObject.KeyItemList.Count == 1);
    }

    [Test]
    public void Store_StoreDefaulBaseKey_KeyListStoredCorrectly()
    {
        var console = new Mock<IConsole>();
        
        var testObject = new KeyStore(console.Object);
        testObject.Clear();
        testObject.Store(_uid1, testObject.DefaultBaseKey);
        var defaultKey = testObject.Find(_uid1);
        testObject.Store(_uid1, _key1);
        var key1 = testObject.Find(_uid1);
        
        Assert.IsNotNull(testObject.KeyItemList);
        Assert.That(testObject.KeyItemList.Count == 1);
        Assert.That(defaultKey.SequenceEqual(testObject.DefaultBaseKey));
        Assert.That(key1.SequenceEqual(_key1));
    }

    [Test]
    public void Find_LoadDatabaseAndFind_FindsAllKeyItems()
    {
        var console = new Mock<IConsole>();
        
        var testObject = new KeyStore(console.Object);
        testObject.Clear();
        testObject.Store(_uid1, _key1);        
        testObject.Store(_uid2, _key2);        
        testObject.Store(_uid3, _key3);        
        testObject.Store(_uid4, _key4);

        var retrievedKey1 = testObject.Find(_uid1);
        var retrievedKey2 = testObject.Find(_uid2);
        var retrievedKey3 = testObject.Find(_uid3);
        var retrievedKey4 = testObject.Find(_uid4);
        
        Assert.IsNotNull(testObject.KeyItemList);
        Assert.That(testObject.KeyItemList.Count == 4);
        Assert.That(retrievedKey1.SequenceEqual(_key1));
        Assert.That(retrievedKey2.SequenceEqual(_key2));
        Assert.That(retrievedKey3.SequenceEqual(_key3));
        Assert.That(retrievedKey4.SequenceEqual(_key4));
    }

    [Test]
    public void Find_LookForMissingUid_FindsAllKeyItems()
    {
        var console = new Mock<IConsole>();
        
        var testObject = new KeyStore(console.Object);
        testObject.Clear();
        testObject.Store(_uid1, _key1);        
        testObject.Store(_uid2, _key2);        
        testObject.Store(_uid3, _key3);        

        var retrievedKey4 = testObject.Find(_uid4);
        
        Assert.IsNotNull(testObject.KeyItemList);
        Assert.That(testObject.KeyItemList.Count == 3);
        Assert.IsNull(retrievedKey4);
    }

    [Test]
    public void List_LoadDatabaseAndList_OutputGoesToConsole()
    {
        var console = new Mock<IConsole>();
        console.Setup(foo => foo.Out.WriteLine(It.IsAny<string>()));

        var testObject = new KeyStore(console.Object);
        testObject.Clear();
        testObject.Store(_uid1, _key1);        
        testObject.Store(_uid2, _key2);        
        testObject.Store(_uid3, _key3);        
        testObject.Store(_uid4, _key4);

        testObject.List();

        Assert.Pass();
    }
    
}
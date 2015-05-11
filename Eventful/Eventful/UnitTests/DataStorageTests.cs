using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eventful.Model;
using System.IO;

namespace Eventful.UnitTests
{
    [TestClass]
    public class DataStorageTests
    {
        [TestMethod]
        public void LoadEventFromDisk()
        {
            string filename = @"C:\Users\alexandros_gouvatsos\Dropbox\Development\LoD\Eventful\No category\Cold Sweat #dc3645a6-b71b-4627-8666-0fd120cde12d.event";
            Assert.IsNotNull(DataStorage.LoadEventFromDisk(filename));
            filename = @"C:\Users\alexandros_gouvatsos\No category\Cold Sweat #dc3645a6-b71b-4627-8666-0fd120cde12d.event";
            Assert.IsNull(DataStorage.LoadEventFromDisk(filename));
            filename = @"";
            Assert.IsNull(DataStorage.LoadEventFromDisk(filename));
            filename = null;
            Assert.IsNull(DataStorage.LoadEventFromDisk(filename));
        }

        [TestMethod]
        public void LoadDeckFromDisk()
        {
            string pathname = @"C:\Users\alexandros_gouvatsos\Dropbox\Development\LoD\Eventful\";
            string deckname = ".Backups";
            Assert.IsNull(DataStorage.LoadDeckFromDisk(deckname, pathname));
            deckname = "No category";
            Assert.IsNotNull(DataStorage.LoadDeckFromDisk(deckname, pathname));
            Assert.IsNull(DataStorage.LoadDeckFromDisk("", ""));
            Assert.IsNull(DataStorage.LoadDeckFromDisk(null, null));
        }

        [TestMethod]
        public void SaveDeckToDisk()
        {
            string pathname = @"MG:\Windows\System32\";
            Deck deck = new Deck(@"UnitTestDeck\");
            Assert.IsFalse(DataStorage.SaveDeckToDisk(deck, pathname));
            Assert.IsFalse(Directory.Exists(String.Concat(pathname, deck.Title)));

            pathname = @"";
            Assert.IsTrue(DataStorage.SaveDeckToDisk(deck, pathname));
            Assert.IsTrue(Directory.Exists(String.Concat(pathname, deck.Title)));
            Directory.Delete(String.Concat(pathname, deck.Title));

            deck = null;
            pathname = null;
            Assert.IsFalse(DataStorage.SaveDeckToDisk(deck, pathname));
            Assert.IsFalse(Directory.Exists(null));

            deck = new Deck("");
            pathname = "";
            Assert.IsFalse(DataStorage.SaveDeckToDisk(deck, pathname));
            Assert.IsFalse(Directory.Exists(String.Concat(pathname, deck.Title)));
        }

        [TestMethod]
        public void SaveEventToDisk()
        {
            string pathname = @"MG:\Windows\System32\";
            Deck deck = new Deck(@"UnitTestDeck\");
            Event ev = new Event("Event");
            Assert.IsFalse(DataStorage.SaveEventToDisk(ev, deck, pathname));
            Assert.IsFalse(Directory.Exists(String.Concat(pathname, deck.Title)));

            pathname = "";
            deck = null;
            ev = new Event("Event");
            Assert.IsFalse(DataStorage.SaveEventToDisk(ev, deck, pathname));

            pathname = "";
            deck = new Deck("hey");
            ev = null;
            Assert.IsFalse(DataStorage.SaveEventToDisk(ev, deck, pathname));

            pathname = "";
            deck = new Deck(@"UnitTestDeck\");
            ev = new Event("Event");
            Assert.IsTrue(DataStorage.SaveDeckToDisk(deck, pathname));
            Assert.IsTrue(Directory.Exists(String.Concat(pathname, deck.Title)));
            Directory.Delete(String.Concat(pathname, deck.Title));

            deck = null;
            ev = null;
            pathname = null;
            Assert.IsFalse(DataStorage.SaveDeckToDisk(deck, pathname));
            Assert.IsFalse(Directory.Exists(null));

            pathname = @"";
            deck = new Deck(@"");
            ev = new Event("");
            //Assert.IsFalse(DataStorage.SaveEventToDisk(ev, deck, pathname));
            //Assert.IsFalse(Directory.Exists(String.Concat(pathname, deck.Title)));
        }
    }
}

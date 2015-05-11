using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Eventful.Model
{
    public static class DataStorage
    {
        public static string DefaultStorageDirectory = String.Concat(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"\Eventful\");

        public static bool SaveEventToDisk(Event ev, Deck deck, string storageDirectory)
        {
            if (deck == null || ev == null || storageDirectory == null) return false;
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Event));
                TextWriter textWriter = new StreamWriter(String.Concat(storageDirectory, RemoveIllegalCharactersFromFilename(deck.Title), @"\", RemoveIllegalCharactersFromFilename(ev.Title), " #", ev.Id, ".event"));
                serializer.Serialize(textWriter, ev);
                textWriter.Close();
                return true;
            }
            catch (DirectoryNotFoundException)
            {
                try
                {
                    SaveDeckToDisk(deck, storageDirectory);
                    SaveEventToDisk(ev, deck, storageDirectory);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
        public static bool SaveDeckToDisk(Deck deck, string storageDirectory)
        {
            try
            {
                Directory.CreateDirectory(String.Concat(storageDirectory, RemoveIllegalCharactersFromFilename(deck.Title)));
                return true;
            }
            catch (DirectoryNotFoundException)
            {
                try
                {
                    Directory.CreateDirectory(storageDirectory);
                    SaveDeckToDisk(deck, storageDirectory);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public static Event LoadEventFromDisk(string filename)
        {
            try
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(Event));
                TextReader textReader = new StreamReader(filename);
                Event ev;
                ev = (Event)deserializer.Deserialize(textReader);
                textReader.Close();
                ev.IsChanged = false;
                return ev;
            }
            catch
            {
                return null;
            }
        }
        public static Deck LoadDeckFromDisk(string deckTitle, string storageDirectory)
        {
            string fullpath = String.Concat(storageDirectory, deckTitle);
            if (!Directory.Exists(fullpath)) return null;
            if (deckTitle.Length <= 0) return null;
            if (deckTitle[0] == '.') return null;
            Deck newDeck = new Deck(deckTitle);
            foreach (string file in Directory.EnumerateFiles(fullpath, "*.event"))
            {
                Event newEvent = LoadEventFromDisk(file);
                if (newEvent != null)
                    newDeck.Events.Add(newEvent);
            }
            return newDeck;
        }
        public static List<Deck> LoadAllDecksFromDisk(string storageDirectory)
        {
            List<Deck> decks = new List<Deck>();
            try
            {
                foreach (string directory in Directory.EnumerateDirectories(storageDirectory))
                {
                    string subDirectory = directory.Replace(storageDirectory, "");
                    Deck deck = LoadDeckFromDisk(subDirectory, storageDirectory);
                    if (deck != null)
                        decks.Add(deck);
                }
                return decks;
            }
            catch
            {
                return new List<Deck>();
            }
        }

        public static void DeleteEvent(Event ev, Deck deck, string storageDirectory)
        {
            BackupEvent(ev, deck, storageDirectory);
        }
        public static void DeleteDeck(Deck deck, string StorageDirectory)
        {
            BackupDeck(deck, StorageDirectory);
        }

        private static void BackupEvent(Event ev, Deck deck, string storageDirectory)
        {
            try
            {
                Directory.CreateDirectory(String.Concat(storageDirectory, RemoveIllegalCharactersFromFilename(deck.Title), @"\", ".Backups"));
                Directory.Move(String.Concat(storageDirectory, RemoveIllegalCharactersFromFilename(deck.Title), @"\", RemoveIllegalCharactersFromFilename(ev.Title), " #", ev.Id, ".event"), String.Concat(storageDirectory, RemoveIllegalCharactersFromFilename(deck.Title), @"\", @".Backups\", String.Concat(RemoveIllegalCharactersFromFilename(ev.Title), " #", ev.Id, " "), RemoveIllegalCharactersFromFilename(DateTime.Now.ToString()), ".event"));
            }
            catch
            {
            }
        }
        private static void BackupDeck(Deck deck, string storageDirectory)
        {
            try
            {
                Directory.CreateDirectory(String.Concat(storageDirectory, ".Backups"));
                Directory.Move(String.Concat(storageDirectory, RemoveIllegalCharactersFromFilename(deck.Title), @"\"), String.Concat(storageDirectory, @".Backups\", RemoveIllegalCharactersFromFilename(deck.Title), " ", RemoveIllegalCharactersFromFilename(DateTime.Now.ToString())));
            }
            catch
            {
            }
        }

        public static void RenameDeck(Deck deck, string newDeckTitle, string storageDirectory)
        {
            try
            {
                Directory.Move(
                    String.Concat(storageDirectory, RemoveIllegalCharactersFromFilename(deck.Title), @"\"),
                    String.Concat(storageDirectory, newDeckTitle, @"\")
                );
            }
            catch
            {
            }
        }

        private static string RemoveIllegalCharactersFromFilename(string filename)
        {
            string[] restrictedStrings = new string[] { "\\", "/", ":", "*", "?", "\"", "<", ">", "|" };
            foreach (string restrictedString in restrictedStrings)
                filename = filename.Replace(restrictedString, "");
            return filename;
        }
    }
}

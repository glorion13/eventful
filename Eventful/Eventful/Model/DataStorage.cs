using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Eventful.Model
{
    public static class DataStorage
    {
        public static string DefaultStorageDirectory = String.Concat(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"\Eventful\");
        public static string StorageDirectory = DefaultStorageDirectory;

        private static string deckDirectoryName = "decks\\";
        private static string tagDirectoryName = "tags\\";
        private static string variableDirectoryName = "variables\\";

        private static bool CreateInitialDirectories()
        {
            try
            {
                Directory.CreateDirectory(StorageDirectory);
                Directory.CreateDirectory(String.Concat(StorageDirectory, deckDirectoryName));
                Directory.CreateDirectory(String.Concat(StorageDirectory, tagDirectoryName));
                Directory.CreateDirectory(String.Concat(StorageDirectory, variableDirectoryName));
                return true;
            }
            catch
            {
                System.Windows.MessageBox.Show("Problem reading/writing to disk. There is a chance your data will not be saved.");
                return false;
            }
        }

        private static string GetEventFileName(Event ev)
        {
            return String.Concat(
                ev.Title,
                " #",
                ev.Id,
                ".event");
        }

        private static string GetDeckFilePath(Deck deck)
        {
            return String.Concat(
                StorageDirectory,
                deckDirectoryName,
                deck.Title,
                @"\");
        }

        public static bool SaveEventToDisk(Event ev, Deck deck)
        {
            if (deck == null || ev == null || StorageDirectory == null) return false;
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Event));
                TextWriter textWriter = new StreamWriter(String.Concat(GetDeckFilePath(deck), GetEventFileName(ev)));
                serializer.Serialize(textWriter, ev);
                textWriter.Close();
                return true;
            }
            catch (DirectoryNotFoundException)
            {
                try
                {
                    SaveDeckToDisk(deck);
                    SaveEventToDisk(ev, deck);
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
        public static bool SaveDeckToDisk(Deck deck)
        {
            try
            {
                Directory.CreateDirectory(GetDeckFilePath(deck));
                return true;
            }
            catch (DirectoryNotFoundException)
            {
                try
                {
                    CreateInitialDirectories();
                    SaveDeckToDisk(deck);
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

        

        public static Event LoadEventFromDisk(string path)
        {
            string fullpath = String.Concat(StorageDirectory, path);
            try
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(Event));
                TextReader textReader = new StreamReader(fullpath);
                Event ev = new Event();
                ev = (Event)deserializer.Deserialize(textReader);
                textReader.Close();
                return ev;
            }
            catch
            {
                return null;
            }
        }
        
        public static async void LoadDeckEventsFromDisk(Deck deck)
        {
            deck.Events.Clear();
            string fullpath = GetDeckFilePath(deck);
            try
            {
                foreach (string file in await Task.Run(() => EnumerateFilesTask(fullpath, "*.event")))
                {
                    string subDirectory = file.Replace(StorageDirectory, "");
                    Event newEvent = await Task.Run(() => LoadEventFromDisk(subDirectory));
                    //newEvent.IsChanged = false;
                    if (newEvent != null)
                        deck.Events.Add(newEvent);
                }
            }
            catch
            {
            }
        }

        private static IEnumerable<string> EnumerateFilesTask(string dir, string filetype)
        {
            try
            {
                return Directory.EnumerateFiles(dir, filetype);
            }
            catch
            {
                return new List<string>();
            }
        }

        private static Deck CreateDeckMapping(string deckMapping)
        {
            string fullpath = String.Concat(StorageDirectory, deckMapping);
            deckMapping = deckMapping.Replace(deckDirectoryName, "");
            if (!Directory.Exists(fullpath)) return null;
            if (deckMapping.Length <= 0) return null;
            if (deckMapping[0] == '.') return null;
            return new Deck(deckMapping);
        }
        public static async void LoadAllDeckMappingsFromDisk(ObservableCollection<Deck> decks)
        {
            decks.Clear();
            try
            {
                foreach (string folder in await Task.Run(() => EnumerateDirectoriesTask(String.Concat(StorageDirectory, deckDirectoryName))))
                {
                    string subDirectory = folder.Replace(StorageDirectory, "");
                    Deck deck = CreateDeckMapping(subDirectory);
                    if (deck != null)
                        decks.Add(deck);
                }
            }
            catch (DirectoryNotFoundException)
            {
                CreateInitialDirectories();
            }
            catch
            {
            }
        }

        private static IEnumerable<string> EnumerateDirectoriesTask(string dir)
        {
            try
            {
                return Directory.EnumerateDirectories(dir);
            }
            catch
            {
                return new List<string>();
            }
        }

        public static bool DeleteEvent(Event ev, Deck deck)
        {
            return BackupEvent(ev, deck);
        }
        public static bool DeleteDeck(Deck deck)
        {
            return BackupDeck(deck);
        }

        private static string GetDateTime()
        {
            return DateTime.Now.ToString().Replace("/", "-").Replace(":", "-");
        }

        private static bool BackupEvent(Event ev, Deck deck)
        {
            try
            {
                Directory.CreateDirectory(String.Concat(GetDeckFilePath(deck), ".Backups"));
                Directory.Move(
                    String.Concat(GetDeckFilePath(deck), GetEventFileName(ev)),
                    String.Concat(
                        GetDeckFilePath(deck),
                        @".Backups\",
                        GetDateTime(),
                        " ",
                        GetEventFileName(ev))
                    );
                return true;
            }
            catch
            {
                return false;
            }
        }
        private static bool BackupDeck(Deck deck)
        {
            try
            {
                string deckBackupsDirectory = String.Concat(StorageDirectory, deckDirectoryName, ".Backups\\");
                Directory.CreateDirectory(deckBackupsDirectory);
                Directory.Move(
                    GetDeckFilePath(deck),
                    String.Concat(deckBackupsDirectory, deck.Title, " ", GetDateTime())
                    );
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool RenameDeck(Deck deck, string newDeckTitle)
        {
            try
            {
                Directory.Move(
                    GetDeckFilePath(deck),
                    String.Concat(StorageDirectory, deckDirectoryName, newDeckTitle, @"\")
                );
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool RenameEvent(Event ev, Deck deck, string newEventTitle)
        {
            try
            {
                Event newEvent = new Event(ev);
                newEvent.Title = newEventTitle;
                newEvent.Id = ev.Id;
                newEvent.IsChanged = false;
                bool success = SaveEventToDisk(newEvent, deck);
                if (success)
                    DeleteEvent(ev, deck);
                return success;
            }
            catch
            {
                return false;
            }
        }
    }
}

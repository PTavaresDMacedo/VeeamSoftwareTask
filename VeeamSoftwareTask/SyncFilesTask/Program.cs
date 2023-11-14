using System.Diagnostics;

namespace SyncFilesTask
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("|Syncronization Program|");

            //provide the information required: source folder, replica folder, interval of syncronization and log file path
            Console.WriteLine("Input the address for the source folder: ");
            string source = Console.ReadLine();
            Console.WriteLine("Input the address for the replica folder: ");
            string replica = Console.ReadLine();
            if (source.Equals(replica))
            {
                Console.WriteLine("You have selected the same folder!");
                return;
            }

            Console.WriteLine("Input the frequency for syncronization (in seconds): ");
            int frequency = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Input the log file path: ");
            string logPath = Console.ReadLine();


            Console.Clear();
            Console.WriteLine("Starting syncronization.");

            Console.WriteLine("Press ESC to stop.");

            //try/catch to run the SyncPeriodically method and test for possible exceptions
            try
            {
                using (StreamWriter log = new StreamWriter(String.Concat(logPath,"\\log.txt"), true))
                {

                    SyncPeriodically(source, replica, frequency, log);

                    Console.WriteLine("Syncronization has stopped.");

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Oops, there was an error: {ex.Message}");
            }

        }

        //method to sync the folders. First check if directory exists, otherwise create it
        public static void SyncFolders(string source, string replica, StreamWriter log)
        {
            if (!Directory.Exists(replica))
            {
                Directory.CreateDirectory(replica);
            }

            string[] replicaFiles = Directory.GetFiles(replica);
            string[] sourceFiles = Directory.GetFiles(source);

            //check every file to see if they exist in source folder but not in the replica. In case not, copy from the source to the replica
            foreach (string sourceFile in sourceFiles)
            {
                string name = Path.GetFileName(sourceFile);
                string replicaPath = Path.Combine(replica, name);

                if (!File.Exists(replicaPath) || File.GetLastWriteTime(sourceFile) > File.GetLastWriteTime(replicaPath))
                {
                    File.Copy(sourceFile, replicaPath, true);
                    log.WriteLine($"{DateTime.Now} : {sourceFile} was copied to the {replicaPath} folder");
                }
            }

            //check every file to see if they exist in replica folder but not in source. In case not, delete from replica
            foreach (string replicaFile in replicaFiles)
            {
                string name = Path.GetFileName(replicaFile);
                string sourcePath = Path.Combine(source, name);

                if (!File.Exists(sourcePath))
                {
                    File.Delete(replicaFile);
                    log.WriteLine($"{DateTime.Now} : {replicaFile} was deleted from the folder");
                }
            }
        }

        //method to establish the interval of syncronization. The method will stop when the "esc" key is pressed
        public static void SyncPeriodically(string source, string replica, int frequency, StreamWriter log)
        {
            while (!(Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape))
            {
                SyncFolders(source, replica, log);
                Console.WriteLine("Syncronizing...");
                Thread.Sleep(frequency * 1000);
            }
        }
    }
}
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using System;
using System.IO;
using System.Security.Permissions;
using System.Threading.Tasks;

namespace blob_quickstart
{
    class Program
    {
        // VALORES DE CONFIGURACION //
        private const string contenedorNombre = "textosprueba1";
        private const string ruta = "C://Seguimiento";
        private const string extension = "*.TXT";
        public static void Main()
        {
            // Ajuste la cantidad de guiones (-) para que coincidan.
            Console.WriteLine("------------------------------------------------------- \n");
            Console.WriteLine("--\\ Importer3a - (Inserte aquí nombre del cliente //-- \n");
            Console.WriteLine("------------------------------------------------------- \n");
            Console.WriteLine("\n");
            Run();
            //Console.WriteLine("Press any key to exit the sample application.");
            //Console.ReadLine();
        }
        /* --------------------------------------------------------------------------------------- */
        // Subirá todos los archivos de la carpeta.
        private static async Task ProcessAsync()
        {
            // Variable de Entorno para la conexión con el servicio
            Console.WriteLine("Obteniendo Variable de Entorno...");
            string storageConnectionString = Environment.GetEnvironmentVariable("STORAGE_CONNECTION_STRING");
            Console.WriteLine(" [OK]\n\n");
            Console.WriteLine("Realizando conexión...");
            // Check whether the connection string can be parsed.
            CloudStorageAccount storageAccount;
            if (CloudStorageAccount.TryParse(storageConnectionString, out storageAccount))
            {
                Console.WriteLine("[OK]\n\n");
                // If the connection string is valid, proceed with operations against Blob
                // storage here.

                // Create the CloudBlobClient that represents the 
                // Blob storage endpoint for the storage account.
                CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();

                // Create a container called 'quickstartblobs' and 
                // append a GUID value to it to make the name unique.
                CloudBlobContainer cloudBlobContainer =
                    cloudBlobClient.GetContainerReference(contenedorNombre); // Nombre del contenedor

                // SI NO EXISTE EL CONTENEDOR LO CREA, SI EXISTE MODIFICA SU CONTENIDO
                await cloudBlobContainer.CreateIfNotExistsAsync();

                // Set the permissions so the blobs are public.
                BlobContainerPermissions permissions = new BlobContainerPermissions
                {
                    PublicAccess = BlobContainerPublicAccessType.Blob
                };
                await cloudBlobContainer.SetPermissionsAsync(permissions);

                // AQUI SE CAMBIA EL DIRECTORIO DONDE SE ENCUENTRA LA CARPETA QUE SE QUIERE SUBIR SU CONTENIDO Y EL TIPO DE ARCHIVO.
                string[] files = Directory.GetFiles(ruta, extension);
                Console.WriteLine("Subiendo archivos de " + ruta + "\n\n");
                foreach (string filePath in files)
                {
                    string fileName = Path.GetFileName(filePath);
                    Console.WriteLine("Archivo = {0}", filePath);
                    Console.WriteLine();
                    Console.WriteLine("Subiendo a Blob storage como blob '{0}'...", fileName);
                    Console.WriteLine();
                    // Get a reference to the blob address, then upload the file to the blob.
                    // Use the value of localFileName for the blob name.
                    CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(fileName);
                    await cloudBlockBlob.UploadFromFileAsync(filePath);
                }

                textoInicio();

            }
            else
            {
                Console.WriteLine("[ERROR]\n\n");
                // Otherwise, let the user know that they need to define the environment variable.
                Console.WriteLine(
                    "La conexión falló.\n " +
                    "Puede ser por las siguiente opciones:\n" +
                    "  - Variable de Entorno no definida. \n" +
                    "  - Variable de Entorno incorrecta o caduca. \n\n" +
                    "En caso de la primera opción, Añada una Variable de Entorno de Sistema llamada 'STORAGE_CONNECTION_STRING'\n" +
                    " y como valor añada la key Connection String. Ejemplo: 'DefaultEndpointsProtocol=https;AccountName=triplealpha;AccountKey=fQsntF2...'\n\n");
                Console.WriteLine("Pulse cualquier tecla para salir de la aplicación.");
                Console.ReadLine();
            }
        }
        /* --------------------------------------------------------------------------------------- */


        /* --------------------------------------------------------------------------------------- */
        // Subirá solo los modificados.
        private static async Task ProcessAsync(string fileOld, string fileNew)
        {
            // Variable de Entorno para la conexión con el servicio
            Console.WriteLine("Obteniendo Variable de Entorno...");
            string storageConnectionString = Environment.GetEnvironmentVariable("STORAGE_CONNECTION_STRING");
            Console.WriteLine(" [OK]\n\n");
            Console.WriteLine("Realizando conexión...");
            // Check whether the connection string can be parsed.
            CloudStorageAccount storageAccount;
            if (CloudStorageAccount.TryParse(storageConnectionString, out storageAccount))
            {
                // If the connection string is valid, proceed with operations against Blob
                // storage here.
                // ADD OTHER OPERATIONS HERE
                // Create the CloudBlobClient that represents the 
                // Blob storage endpoint for the storage account.
                CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();

                // Create a container called 'quickstartblobs' and 
                // append a GUID value to it to make the name unique.
                CloudBlobContainer cloudBlobContainer =
                    cloudBlobClient.GetContainerReference(contenedorNombre); // Nombre del contenedor
                // Guid.NewGuid().ToString());
                // await cloudBlobContainer.CreateAsync();

                // SI NO EXISTE EL CONTENEDOR LO CREA, SI EXISTE MODIFICA SU CONTENIDO
                await cloudBlobContainer.CreateIfNotExistsAsync();

                // Set the permissions so the blobs are public.
                BlobContainerPermissions permissions = new BlobContainerPermissions
                {
                    PublicAccess = BlobContainerPublicAccessType.Blob
                };
                await cloudBlobContainer.SetPermissionsAsync(permissions);

                // Create a file in your local MyDocuments folder to upload to a blob.
                /*
                string localPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string localFileName = "QuickStart_" + Guid.NewGuid().ToString() + ".txt";
                string sourceFile = Path.Combine(localPath, localFileName);
                */
                // Write text to the file.
                // File.WriteAllText(sourceFile, "Hello, World!");

                // AQUI SE CAMBIA EL DIRECTORIO DONDE SE ENCUENTRA LA CARPETA QUE SE QUIERE SUBIR SU CONTENIDO Y EL TIPO DE ARCHIVO.
                // string[] files = Directory.GetFiles(ruta, extension);
                if (fileOld.Equals(fileNew))
                {
                    string sourceFile = Path.Combine(ruta, fileNew);

                    Console.WriteLine("Subiendo a Blob storage como blob '{0}'...", fileNew);
                    CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(fileNew);
                    await cloudBlockBlob.UploadFromFileAsync(sourceFile);
                    Console.WriteLine("[OK]\n");
                }
                else
                {
                    string sourceFile = Path.Combine(ruta, fileNew);

                    Console.WriteLine("sustituyendo " + fileOld + " como " + fileNew + " a Blob storage como blob...");
                    CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(fileOld);
                    await cloudBlockBlob.DeleteIfExistsAsync();

                    cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(fileNew);
                    await cloudBlockBlob.UploadFromFileAsync(sourceFile);
                    Console.WriteLine("[OK]");

                    textoInicio();
                }

            }
            else
            {
                Console.WriteLine("[ERROR]\n\n");
                // Otherwise, let the user know that they need to define the environment variable.
                Console.WriteLine(
                    "La conexión falló.\n " +
                    "Puede ser por las siguiente opciones:\n" +
                    "  - Variable de Entorno no definida. \n" +
                    "  - Variable de Entorno incorrecta o caduca. \n\n" +
                    "En caso de la primera opción, Añada una Variable de Entorno de Sistema llamada 'STORAGE_CONNECTION_STRING'\n" +
                    " y como valor añada la key Connection String. Ejemplo: 'DefaultEndpointsProtocol=https;AccountName=triplealpha;AccountKey=fQsntF2...'\n\n");
                Console.WriteLine("Pulse cualquier tecla para salir de la aplicación.");
                Console.ReadLine();
            }
        }
        /* --------------------------------------------------------------------------------------- */


        // SE ENCARGA DE NOTIFICAR CUALQUIER CAMBIO EN LA CARPETA DE DESTINO

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        private static void Run()
        {
            // Create a new FileSystemWatcher and set its properties.
            using (FileSystemWatcher watcher = new FileSystemWatcher())
            {
                //watcher.Path = args[1];
                watcher.Path = ruta;
                // Watch for changes in LastAccess and LastWrite times, and
                // the renaming of files or directories.
                watcher.NotifyFilter = NotifyFilters.LastAccess
                                     | NotifyFilters.LastWrite
                                     | NotifyFilters.FileName
                                     | NotifyFilters.DirectoryName;

                // Only watch text files.
                // SOLO ESCUCHA ESTE TIPO DE ARCHIVO, EN CASO DE QUERER TODOS LOS ARCHIVOS, DEJAR VACIO ESTE CAMPO.
                watcher.Filter = extension;

                // Add event handlers.
                watcher.Changed += OnChanged;
                watcher.Created += OnCreated;
                watcher.Deleted += OnDeleted;
                watcher.Renamed += OnRenamed;

                // Begin watching.
                watcher.EnableRaisingEvents = true;

                // Wait for the user to quit the program.
                Console.WriteLine("----------------------------------------");
                Console.WriteLine("- Opciones |'1' Subir Todo | 'q' Salir -");
                Console.WriteLine("----------------------------------------");
                if (Console.Read() == '1')
                {
                    ProcessAsync().GetAwaiter().GetResult(); ;
                }
                while (Console.Read() != 'q') ;
            }
        }

        private static void textoInicio()
        {
            Console.WriteLine("----------------------------------------");
            Console.WriteLine("- Opciones |'1' Subir Todo | 'q' Salir -");
            Console.WriteLine("----------------------------------------");
        }

        // Define the event handlers.
        private static void OnChanged(object source, FileSystemEventArgs e) //=>
        {
            // Specify what is done when a file is changed, created, or deleted.
            ProcessAsync(e.Name, e.Name).GetAwaiter().GetResult();
            Console.WriteLine($"Archivo: {e.FullPath} {e.ChangeType} \n\n");

            textoInicio();
        }

        private static void OnCreated(object source, FileSystemEventArgs e) //=>
        {
            // Specify what is done when a file is changed, created, or deleted.
            ProcessAsync(e.Name, e.Name).GetAwaiter().GetResult();
            Console.WriteLine($"Archivo: {e.FullPath} {e.ChangeType} \n\n");

            textoInicio();
        }

        private static void OnDeleted(object source, FileSystemEventArgs e) //=>
        {
            Console.WriteLine($"Archivo: {e.Name} {e.ChangeType} \n\n");

            textoInicio();
        }

        private static void OnRenamed(object source, RenamedEventArgs e) //=>
        {
            // Specify what is done when a file is renamed.
            ProcessAsync(e.OldName, e.Name).GetAwaiter().GetResult();
            Console.WriteLine($"Archivo: {e.OldFullPath} renombrado a {e.FullPath} \n\n");

            textoInicio();
        }

    }
}
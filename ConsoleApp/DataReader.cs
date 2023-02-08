namespace ConsoleApp
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    public class DataReader
    {
        //IEnumerable<ImportedObject> ImportedObjects;

        //1. Zamiast 2 klas ImportedObjectBaseClass i ImportedObject wystarczy jedna ImportedObject - publiczna
        //   z wszystkimi polami jako własciwościami i z warościami domyślnymi. W oryginale element Name z klasy ImportedOblect przykrywał element klasy bazowej
        //2. Trzeba kontrolować, czy importowana linia dzieli się faktycznie na 7 elementów oddzielonych średnikiem.
        //3. Jest błąd w pętli czytania listy, <= zastąpić należy przez <, ale można wyeliminować początkowe w wczytywanie linii pliku do listy
        //4. Funkcje w pętli opisanej jako "clear and correct imported data" można zrealizowac od razu przy czytaniu danych - jeden przebieg mniej
        //5. Metody zostają rozdzielone na czytanie danych i wydruk danych. Lista danych jest publiczna
        //   PrintData przeciążona - może drukować zewnętrzną listę lub zapisaną w klasie.

        public IEnumerable<ImportedObject> ImportedObjects = new List<ImportedObject>() { new ImportedObject() };

        public IEnumerable<ImportedObject> ImportData(string fileToImport)
        {
            var streamReader = new StreamReader(fileToImport);

            //var importedLines = new List<string>();
            while (!streamReader.EndOfStream)
            {
                var line = streamReader.ReadLine();
                var values = line.TrimEnd().Split(';');
                if (values.Length < 7)
                    continue;
                var importedObject = new ImportedObject();
                importedObject.Type = values[0].Trim().Replace(" ", "").Replace(Environment.NewLine, "").ToUpper(); 
                importedObject.Name = values[1].Trim().Replace(" ", "").Replace(Environment.NewLine, "").ToUpper();
                importedObject.Schema = values[2].Trim().Replace(" ", "").Replace(Environment.NewLine, "").ToUpper(); ;
                importedObject.ParentName = values[3].Trim().Replace(" ", "").Replace(Environment.NewLine, "").ToUpper(); 
                importedObject.ParentType = values[4].Trim().Replace(" ", "").Replace(Environment.NewLine, "").ToUpper(); 
                importedObject.DataType = values[5].Trim().Replace(" ", "").Replace(Environment.NewLine, "").ToUpper(); ;
                importedObject.IsNullable = values[6];
                ((List<ImportedObject>)ImportedObjects).Add(importedObject);

                //importedLines.Add(line);
            }

            //for (int i = 0; i < importedLines.Count; i++)
            //{
            //    var importedLine = importedLines[i];
            //    var values = importedLine.Split(';');
            //    if (values.Length < 7)
            //        continue;
            //    var importedObject = new ImportedObject();
            //    importedObject.Type = values[0];
            //    importedObject.Name = values[1];
            //    importedObject.Schema = values[2];
            //    importedObject.ParentName = values[3];
            //    importedObject.ParentType = values[4];
            //    importedObject.DataType = values[5];
            //    importedObject.IsNullable = values[6];
            //    ((List<ImportedObject>)ImportedObjects).Add(importedObject);
            //}

            // clear and correct imported data
            //foreach (var importedObject in ImportedObjects)
            //{
            //    importedObject.Type = importedObject.Type.Trim().Replace(" ", "").Replace(Environment.NewLine, "").ToUpper();
            //    importedObject.Name = importedObject.Name.Trim().Replace(" ", "").Replace(Environment.NewLine, "");
            //    importedObject.Schema = importedObject.Schema.Trim().Replace(" ", "").Replace(Environment.NewLine, "");
            //    importedObject.ParentName = importedObject.ParentName.Trim().Replace(" ", "").Replace(Environment.NewLine, "");
            //    importedObject.ParentType = importedObject.ParentType.Trim().Replace(" ", "").Replace(Environment.NewLine, "");
            //}

            // assign number of children
            for (int i = 0; i < ImportedObjects.Count(); i++)
            {
                var importedObject = ImportedObjects.ToArray()[i];
                foreach (var impObj in ImportedObjects)
                {
                    if (impObj.ParentType == importedObject.Type)
                    {
                        if (impObj.ParentName == importedObject.Name)
                        {
                            importedObject.NumberOfChildren = 1 + importedObject.NumberOfChildren;
                        }
                    }
                }
            }

            return ImportedObjects;
        }

        public void PrintData()
        {
            this.PrintData(this.ImportedObjects);
        }

        public void PrintData(IEnumerable<ImportedObject> rImportedObjects)
        {
            foreach (var database in rImportedObjects)
            {
                if (database.Type == "DATABASE")
                {
                    Console.WriteLine($"Database '{database.Name}' ({database.NumberOfChildren} tables)");

                    // print all database's tables
                    foreach (var table in rImportedObjects)
                    {
                        if (table.ParentType.ToUpper() == database.Type)
                        {
                            if (table.ParentName == database.Name)
                            {
                                Console.WriteLine($"\tTable '{table.Schema}.{table.Name}' ({table.NumberOfChildren} columns)");

                                // print all table's columns
                                foreach (var column in ImportedObjects)
                                {
                                    if (column.ParentType.ToUpper() == table.Type)
                                    {
                                        if (column.ParentName == table.Name)
                                        {
                                            Console.WriteLine($"\t\tColumn '{column.Name}' with {column.DataType} data type {(column.IsNullable == "1" ? "accepts nulls" : "with no nulls")}");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

        }


    }


    public class ImportedObject 
    {
        public string Name { get; set; } = "";
        public string Type { get; set; } = "";

        public string Schema { get; set; } = "";

        public string ParentName { get; set; } = "";
        public string ParentType { get; set; } = "";
        public string DataType { get; set; } = "";
        public string IsNullable { get; set; } = "";

        public double NumberOfChildren { get; set; } = 0;
    }

}

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarregadorDeDados
{
    class Program
    {
        
        static void Main(string[] args)
        {

            var PathFile = "";
            var PathBanco = "";

            Console.WriteLine("Insira o endereço do arquivo");
            PathFile= Console.ReadLine();

            List<string> cabecalho = new List<string>();
            try
            {

                using (var reader = new StreamReader(PathFile))
                {
                    var linha = reader.ReadLine();
                    foreach (var x in linha.Split(';'))
                    {
                        cabecalho.Add(x.Replace("\"", ""));
                    }


                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Problemas ao abrir o arquivo" + ex.Message);
                Console.ReadKey();

             }

            var queryGenerated = GetQuery(cabecalho);
            Console.WriteLine(queryGenerated);
            Console.WriteLine("Insira a string de conexão");
            PathBanco = Console.ReadLine();
            
            try
            {
                using (SqlConnection connection = new SqlConnection(PathBanco))
                {
                  connection.Open();
                  SqlCommand cmd = new SqlCommand(queryGenerated, connection);
                  cmd.ExecuteNonQuery();
                  Console.WriteLine("Tabela Criada");
                    connection.Close();
                }

            }
            catch (Exception ex)
            {

                Console.WriteLine("Problemas ao executar interações com banco de dados" + ex.Message);
                Console.ReadKey();
            }

            Console.WriteLine("Realizando a carga dos arquivos");


            SqlConnection connect = new SqlConnection(PathBanco);
            try
            {

                using (var reader = new StreamReader(PathFile))
                {
                    var linha = reader.ReadLine();
                    foreach (var x in linha.Split(';'))
                    {
                        cabecalho.Add(x.Replace("\"", ""));
                    }

                      while (!reader.EndOfStream)
                      {
                          var x = reader.ReadLine();

                          Console.WriteLine(AddItem(x.Split(';'), connect));
                      }
    
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey();
            }

            Console.ReadKey();
        }


        private static string GetQuery(List<string> itens)
        {
            var sql = "create table carga (token123);";
            var sqlline = "";

            foreach (var item in itens)
            {
                sqlline += $@"[{item}] varchar(max),";
            }

            sqlline = sqlline.Substring(0, sqlline.Length - 1);
            return sql.Replace("token123", sqlline);
        }


        private static string AddItem(string[] registro, SqlConnection con)
        {

            //Alterando a string

            string commandSql = "insert into carga values (token)";

            string parametros="";
            for (int i =0;i< registro.Length;i++)
            {

                parametros = $"{parametros} @P{i},";

            }
            parametros = parametros.Substring(0, parametros.Length - 1);

            commandSql = commandSql.Replace("token", parametros);

                       
            try
            {

                using (SqlCommand command = new SqlCommand(commandSql, con))
                {


                    for (int i = 0; i < registro.Length; i++)
                    {

                        command.Parameters.AddWithValue($"@P{i}", registro[i]);
                    }

                   Console.WriteLine(command.ToString());


                    con.Open();
                    int result = command.ExecuteNonQuery();

                    if (result < 0)
                        return ("Error");

                    con.Close();

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Problemas ao inserir: "+ ex.Message);
                con.Close();
                Console.ReadKey();
            }

            //sqlline = sqlline.Substring(0, sqlline.Length - 1);
            //return "ok"commandoSql.Replace("token", sqlline);
            return "ok";

        }



    }
}

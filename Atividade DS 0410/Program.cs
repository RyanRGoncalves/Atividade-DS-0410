using System;
using System.Runtime.Remoting.Channels;
using System.Security.Principal;
using MySql.Data.MySqlClient;

namespace Atividade_DS_0410
{
    internal class Program
    {
        static MySqlConnection connection = new MySqlConnection("server=localhost;uid=root;pwd=;database=escola"); // Atributo utilizado para accesar a conexão com o banco. utilização de "Static" para que o atributo seja parte da classe e não de um objeto da classe.
        static void Main() // Primeira função iniciada pelo código. inicializará a conexão e redirecionará o usuário para o Menu principal.
        {
            try // Usado para tentar(try) o bloco abaixo,
            {
                connection.Open(); 
            }
            catch(Exception error) // caso o bloco apresente um erro, este catch irá pegar o erro e mandar a mensagem vinculada a ele para o Console.
            {
                Console.WriteLine(error.Message);
            }
            TrocarPagina(0, "Bem vindo ao gerenciador de alunos!");
        }
        static void TrocarPagina(int numeropagina, string mensagem = null) // Utilizada para... trocar de pagina. 
        {
            Console.Clear(); // Limpa todo o texto dentro to Console.
            if (mensagem != null)
            {
                Console.WriteLine(mensagem+"\n"); // Manda uma mensagem antes de carregar a pagina, caso há alguma mensagem para mandar.
            }
            switch (numeropagina) // Paginas: 0. Menu - 1. Adicionar Aluno - 2. Visualizar/Alterar/Remover Alunos - 4. Sair
            {
                case 0:
                    PaginaMenu();
                    break;
                case 1:
                    PaginaAdicionar();
                    break;
                case 2:
                    PaginaVisualizar();
                    break;
            }
        }
        static void PaginaMenu() // Pagina para guiar o usuário.
        {
            Console.WriteLine("------------       Menu       ------------");
            Console.WriteLine("Pressione o número para a função que deseja accesar."); // Mostrar o menu para o usuário
            Console.WriteLine("1. Adicionar Alunos\n2. Visualizar/alterar Alunoss\n3. Sair do aplicativo");

            if (int.TryParse(Console.ReadLine(), out int escolha) && escolha > 0 && escolha < 3) // Se o usuario colocar 1 ou 2, vai ir para as paginas adicionar e visualisar/alterar/remover, respectivamente.
            {
                TrocarPagina(escolha);
            }
            else if (escolha == 3) // Se o usuário colocar 3, então vai mostrar uma mensagem e quando ele clicar qualquer tecla o aplicativo fecha.
            {
                Console.Clear();
                Console.WriteLine("Obrigado por utilizar este aplicativo! Digite qualquer tecla para sair.\n\t\t- Ryan \"Blank\" Gonçalves");
                Console.ReadKey();
            }
            else // Se ele colocar um valor que não seja um número, 
            {
                TrocarPagina(0, "Erro: Escolha inválida!");
            }
        }
        static void PaginaAdicionar() // Pagina para adicionar um Aluno.
        {
            Console.WriteLine("------------       Adicionar       ------------");
            Console.WriteLine("Informe o nome do Aluno:\n");
            string nome = Console.ReadLine();

            Console.Clear();
            Console.WriteLine("------------       Adicionar       ------------");
            Console.WriteLine("Informe a idade do Aluno:\n"); 
            int idade;
            for (int tentativa=1; !int.TryParse(Console.ReadLine(), out idade); tentativa++)
            {
                if (tentativa%3 != 0)
                {
                    Console.WriteLine("Não foi colocado um valor possivel, tente novamente:\n");
                }
                else
                {
                    Console.WriteLine("Você já tentou "+tentativa+" vezes, deseja continuar? Digite 1 para sim.");
                    if (Console.ReadLine() == "1")
                    {
                        Console.WriteLine("Então coloque um valor possivel (Maior que 0):\n");
                    }
                    else
                    {
                        TrocarPagina(0, "Aluno não adicionado.");
                    }
                }
            }

            Console.Clear();
            Console.WriteLine("------------       Adicionar       ------------");
            Console.WriteLine("Informe o curso do Aluno:\n");
            string curso = Console.ReadLine();

            MySqlCommand commandomysql = new MySqlCommand("INSERT INTO aluno(nome_Aluno, idade_Aluno, curso_Aluno) VALUES (@nome, @idade, @curso);", connection);
            commandomysql.Parameters.AddWithValue("@nome", nome);
            commandomysql.Parameters.AddWithValue("@idade", idade);
            commandomysql.Parameters.AddWithValue("@curso", curso);
            commandomysql.ExecuteNonQuery();

            TrocarPagina(0, $"Aluno {nome} com {idade} anos no curso {curso} adicionado com sucesso!");
        }
        static void PaginaVisualizar(string pesquisa = null)
        {
            Console.WriteLine("------------       Visualizar       ------------");
            Console.WriteLine($"Uma lista de todos os alunos {(pesquisa == null ? "cadastrados" : $"com \"{pesquisa}\" no nome")}, ordenada pelo Id.");
            Console.WriteLine("Digite o Id do aluno para alterar ou remover o aluno e/ou seus cadastros. Digite um nome para encurtar a lista. Digite 0 para voltar ao menu.");
            int numerodealunos = Convert.ToInt32(new MySqlCommand($"SELECT COUNT(*) FROM aluno {(pesquisa == null ? "" : $"WHERE nome_Aluno LIKE '%{pesquisa}%'")};", connection).ExecuteScalar());
            Console.WriteLine($"{numerodealunos} Aluno(s) encontrados.\n");

            if (numerodealunos != 0)
            {
                Console.WriteLine("Id - Nome - Idade - Curso");
                MySqlCommand commandomysql = new MySqlCommand($"SELECT * FROM aluno {(pesquisa == null ? "" : $"WHERE nome_Aluno LIKE '%{pesquisa}%'")};", connection);
                MySqlDataReader reader = commandomysql.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine($"{reader[0]} - {reader[1]} - {reader[2]} - {reader[3]}");
                }
                reader.Close();

                string resposta = Console.ReadLine();
                if (int.TryParse(resposta, out int id))
                {
                    if (id < 0)
                    {
                        TrocarPagina(2, "Valor inválido.");
                    }
                    else if (id == 0)
                    {
                        TrocarPagina(0);
                    }
                    else
                    {
                        Console.Clear();
                        AltRemAluno(id);
                    }
                }
                else
                {
                    Console.Clear();
                    PaginaVisualizar(resposta);
                }
            }
            else
            {
                if (pesquisa == null)
                {
                    Console.WriteLine("Está escola não possui nenhum aluno cadastrado, cadastre alunos utilizando a primeira opção na pagina de menu.");
                    Console.WriteLine("Digite qualquer tecla para voltar ao menu.");
                    Console.ReadKey();
                    TrocarPagina(0);
                }
                else
                {
                    Console.WriteLine($"Não foi encontrado nenhum aluno que contenha \"{pesquisa}\" no nome.");
                    Console.WriteLine("Digite 0 para voltar ao menu ou digite outra linha para fazer outra pesquisa.");
                    string resposta = Console.ReadLine();
                    if (resposta == "0")
                    {
                        TrocarPagina(0);
                    }
                    else
                    {
                        Console.Clear();
                        PaginaVisualizar(resposta);
                    }
                }
            }
        }
        static void AltRemAluno(int id)
        {
            Console.WriteLine("------------       Alterar/Remover       ------------");
            if (Convert.ToInt32(new MySqlCommand($"SELECT COUNT(*) FROM aluno WHERE id_Aluno = {id};", connection).ExecuteScalar()) != 0)
            {
                MySqlCommand commandomysql = new MySqlCommand($"SELECT nome_Aluno, idade_Aluno, curso_Aluno FROM aluno WHERE id_Aluno = {id};", connection);
                MySqlDataReader reader = commandomysql.ExecuteReader();
                reader.Read();

                Console.WriteLine($"Nome: {reader[0]};\nIdade: {reader[1]};\nCurso: {reader[2]}.\n");
                Console.WriteLine("Digite qual campo deseja alterar. Digite 0 para voltar ao menu. Digite \"rem\" para remover o aluno.");
                string resposta = Console.ReadLine().ToLower();
                string nome = reader[0].ToString();

                reader.Close();
                if (resposta == "nome" || resposta == "idade" || resposta == "curso")
                {
                    Console.Clear();
                    Console.WriteLine("------------       Alterar       ------------");
                    Console.WriteLine($"Informe o {resposta} do aluno {nome}:\n");

                    string novocampo;

                    if (resposta != "idade")
                    {
                        novocampo = Console.ReadLine();
                    }
                    else
                    {
                        int idade;
                        for (int tentativa = 1; !int.TryParse(Console.ReadLine(), out idade); tentativa++)
                        {
                            if (tentativa % 3 != 0)
                            {
                                Console.WriteLine("Não foi colocado um valor possivel, tente novamente:\n");
                            }
                            else
                            {
                                Console.WriteLine("Você já tentou " + tentativa + " vezes, deseja continuar? Digite 1 para sim.");
                                if (Console.ReadLine() == "1")
                                {
                                    Console.WriteLine("Então coloque um valor possivel (Maior que 0):\n");
                                }
                                else
                                {
                                    TrocarPagina(0, "Aluno não adicionado.");
                                }
                            }
                        }
                        novocampo = idade.ToString();
                    }

                    commandomysql.CommandText = $"UPDATE aluno SET {resposta}_Aluno = @novocampo";
                    commandomysql.Parameters.AddWithValue("@novocampo", novocampo);
                    commandomysql.ExecuteNonQuery();

                    TrocarPagina(0, $"O {resposta} do aluno \"{nome}\" foi alterado para {novocampo}.");
                }
                else if (resposta == "rem")
                {
                    commandomysql.CommandText = $"DELETE FROM aluno WHERE id_Aluno = {id}";
                    commandomysql.ExecuteNonQuery();

                    TrocarPagina(0, $"O aluno \"{nome}\" foi removido dos dados.");
                }
                else if (resposta == "0")
                {
                    TrocarPagina(0, $"Aluno {nome} não teve alterações.");
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Campo inválido.\n");
                    AltRemAluno(id);
                }
            }
            else
            {
                Console.WriteLine("Não foi possivel encontrar o aluno com Id "+id+"\nDigite qualquer tecla para voltar ao menu.");
                Console.ReadKey();
                TrocarPagina(0);
            }
        }
    }
}

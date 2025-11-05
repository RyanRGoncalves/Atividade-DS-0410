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
                connection.Close();
                Console.ReadKey();
            }
            else // Se ele colocar um valor que não seja um número, então vai "recarregar" a pagina e mostrar a mensagem
            {
                TrocarPagina(0, "Erro: Escolha inválida!");
            }
        }
        static void PaginaAdicionar() // Pagina para adicionar um Aluno.
        {
            Console.WriteLine("------------       Adicionar       ------------");
            Console.WriteLine("Informe o nome do Aluno:\n"); // Designe e solicitação basica
            string nome = Console.ReadLine();

            Console.Clear();
            Console.WriteLine("------------       Adicionar       ------------");
            Console.WriteLine("Informe a idade do Aluno:\n"); // Design e solicitação de números
            int idade;
            for (int tentativa=1; !int.TryParse(Console.ReadLine(), out idade); tentativa++) // este for é usado como contador e para continuar, se o usuario não colocar um número então a condição dará true, continuando o for
            {
                if (tentativa%3 != 0) // % é para pegar o restante de uma divisão, ou seja, aqui toda 3ª tentativa esta condição dará False
                {
                    Console.WriteLine("Não foi colocado um valor possivel, tente novamente:\n");
                }
                else
                {
                    Console.WriteLine("Você já tentou "+tentativa+" vezes, deseja continuar? Digite 1 para sim."); // Se o usuário não querer adicionar um Aluno, então isto é para ele poder sair da pagina de adicionar.
                    if (Console.ReadLine() == "1")
                    {
                        Console.WriteLine("Então coloque um valor possivel (Maior que 0):\n");
                    }
                    else
                    {
                        TrocarPagina(0, "Aluno não adicionado."); // redireciona o usuario de volta para o menu.
                    }
                }
            }

            Console.Clear();
            Console.WriteLine("------------       Adicionar       ------------");
            Console.WriteLine("Informe o curso do Aluno:\n"); // Design e solicitação basica
            string curso = Console.ReadLine();

            MySqlCommand commandomysql = new MySqlCommand("INSERT INTO aluno(nome_Aluno, idade_Aluno, curso_Aluno) VALUES (@nome, @idade, @curso);", connection); // Cria um novo commando mysql, para inserir o novo usuario
            commandomysql.Parameters.AddWithValue("@nome", nome);
            commandomysql.Parameters.AddWithValue("@idade", idade); // Adicionando as variaveis conforme o que o usuario digitou
            commandomysql.Parameters.AddWithValue("@curso", curso);
            commandomysql.ExecuteNonQuery(); // Executa o Insert

            TrocarPagina(0, $"Aluno {nome} com {idade} anos no curso {curso} adicionado com sucesso!"); // redireciona o usuario de volta para o menu.
        }
        static void PaginaVisualizar(string pesquisa = null) // Pagina para visualizar, remover e alterar usuarios (isso vai ser uma droga pra explicar em)
        {
            Console.WriteLine("------------       Visualizar       ------------"); // Design
            Console.WriteLine($"Uma lista de todos os alunos {(pesquisa == null ? "cadastrados" : $"com \"{pesquisa}\" no nome")}, ordenada pelo Id."); // Caso o parametro "pesquisa" estiver nulo, então vai mostrar apenas "cadastrados" nesta mensagem. Caso o contrario então vai mostrar "com {pesquisa} no nome".
            Console.WriteLine("Digite o Id do aluno para alterar ou remover o aluno e/ou seus cadastros. Digite um nome para encurtar a lista. Digite 0 para voltar ao menu."); // Instruções ao usuário
            int numerodealunos = Convert.ToInt32(new MySqlCommand($"SELECT COUNT(*) FROM aluno {(pesquisa == null ? "" : $"WHERE nome_Aluno LIKE '%{pesquisa}%'")};", connection).ExecuteScalar()); // Vai dar um select onde só retorna um valor, mais specificamente isto vai pegar o número de Alunos que tem e vai inserir na variavel "numerodealunos". Também há o uso do mesmo esquema que fiz na linha 110 para quando o usuário pesquisar um nome.
            Console.WriteLine($"{numerodealunos} Aluno(s) encontrados.\n"); // Exibição do número de alunos.

            if (numerodealunos != 0) // Se o número de alunos não for 0, então vai mostrar todos os alunos possiveis
            {
                Console.WriteLine("Id - Nome - Idade - Curso"); // Instrução para o usuário ao como será exibido os dados
                MySqlCommand commandomysql = new MySqlCommand($"SELECT * FROM aluno {(pesquisa == null ? "" : $"WHERE nome_Aluno LIKE '%{pesquisa}%'")};", connection); // Select de todos os alunos possivels, utilizando o mesmo esquema da linha 112 e 110 para se o usuário pesquisar algo.
                MySqlDataReader reader = commandomysql.ExecuteReader(); // O reader serve para armazenar e depois exibir todos os dados que forão resgatados do banco de dados.
                while (reader.Read()) // Este while fará com que o read passará por cada linha possivel.
                {
                    Console.WriteLine($"{reader[0]} - {reader[1]} - {reader[2]} - {reader[3]}"); // Exibição basica de dados
                }
                reader.Close(); // è necessario fechar o reader senão dá erro

                string resposta = Console.ReadLine(); // Leitura de input do usuário
                if (int.TryParse(resposta, out int id)) // Se for número...
                {
                    if (id < 0) // ...e for menor que zero, então vai refrescar a pagina e mandar uma mensagem.
                    {
                        TrocarPagina(2, "Valor inválido.");
                    }
                    else if (id == 0) // ...e for igual a zero, então vai apenas voltar para o menu.
                    {
                        TrocarPagina(0);
                    }
                    else // ...e for um número positivo, então vai limpar o console e ir para pagina de alteração/remoção de usuário.
                    {
                        Console.Clear();
                        AltRemAluno(id);
                    }
                }
                else // Se NÂO for número, então vai "refrescar" a pagina, porém agora com o que o usuário digitou como o parâmetro "resposta".
                {
                    Console.Clear();
                    PaginaVisualizar(resposta);
                }
            }
            else // Se o número de alunos for 0, então...
            {
                if (pesquisa == null) // ... se não tiver nenhuma pesquisa, irá mostrar esta mensagem falando que não há dados no banco.
                {
                    Console.WriteLine("Está escola não possui nenhum aluno cadastrado, cadastre alunos utilizando a primeira opção na pagina de menu.");
                    Console.WriteLine("Digite qualquer tecla para voltar ao menu.");
                    Console.ReadKey();
                    TrocarPagina(0);
                }
                else // ... se tiver alguma pesquisa, irá perguntar ao usuário que ele quer...
                {
                    Console.WriteLine($"Não foi encontrado nenhum aluno que contenha \"{pesquisa}\" no nome.");
                    Console.WriteLine("Digite 0 para voltar ao menu ou digite outra linha para fazer outra pesquisa.");
                    string resposta = Console.ReadLine();
                    if (resposta == "0") // ...voltar para o menu principal...
                    {
                        TrocarPagina(0);
                    }
                    else // ... ou tentar uma outra pesquisa.
                    {
                        Console.Clear();
                        PaginaVisualizar(resposta);
                    }
                }
            }
        }
        static void AltRemAluno(int id) // "Pagina" para alteração e/ou remoção do usuário.
        {
            Console.WriteLine("------------       Alterar/Remover       ------------"); // Design
            if (Convert.ToInt32(new MySqlCommand($"SELECT COUNT(*) FROM aluno WHERE id_Aluno = {id};", connection).ExecuteScalar()) != 0) // Verifica se o número que o usuário colocou possui um id vinculado.
            {
                MySqlCommand commandomysql = new MySqlCommand($"SELECT nome_Aluno, idade_Aluno, curso_Aluno FROM aluno WHERE id_Aluno = {id};", connection); // Pega o nome, a idade e o curso do aluno para mostrar novamente ao usuário.
                MySqlDataReader reader = commandomysql.ExecuteReader(); // Lê o resultado dado pelo comando.
                reader.Read(); // vai uma linha a frente, para que seja mostrado os dados do aluno.

                Console.WriteLine($"Nome: {reader[0]};\nIdade: {reader[1]};\nCurso: {reader[2]}.\n"); // Exibe novamente ao usuário os dados do aluno.
                Console.WriteLine("Digite qual campo deseja alterar. Digite 0 para voltar ao menu. Digite \"rem\" para remover o aluno."); // Instruções para o usuário.
                string resposta = Console.ReadLine().ToLower(); // Pega o que o usuário digitou, coloca tudo em minusculo e guarda em "resposta".
                string nome = reader[0].ToString(); // pega o nome do Aluno e guarda na variavel "nome".

                reader.Close(); // fecha o reader.
                if (resposta == "nome" || resposta == "idade" || resposta == "curso") // Verifica se o usuário escreveu "nome", "idade" ou "curso"
                {
                    Console.Clear();
                    Console.WriteLine("------------       Alterar       ------------"); // Design
                    Console.WriteLine($"Informe o {resposta} do aluno {nome}:\n"); // utilizando a variavel para encurtar o código.

                    string novocampo; // variavel utilizada para armazenar o novo valor.

                    if (resposta != "idade") // Solicitação basica para string.
                    {
                        novocampo = Console.ReadLine();
                    }
                    else // Solicitação de números (é a mesma coisa da pagina de adicionar)
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

                    commandomysql.CommandText = $"UPDATE aluno SET {resposta}_Aluno = @novocampo"; // utiliza a variavel resposta novamente para o código ser mais curto
                    commandomysql.Parameters.AddWithValue("@novocampo", novocampo); // substituindo o @novocampo para ter o valor dento da variavel novocampo
                    commandomysql.ExecuteNonQuery(); // executa o commando

                    TrocarPagina(0, $"O {resposta} do aluno \"{nome}\" foi alterado para {novocampo}."); // manda o usuário para a pagina de menu.
                }
                else if (resposta == "rem") // se, ao invez de "nome", "idade" ou "curso", o usuário escreveu "rem" então vai executar este código.
                {
                    commandomysql.CommandText = $"DELETE FROM aluno WHERE id_Aluno = {id}"; // Deixará o comando de deletação pronto para executar, utilizando a variavel id
                    commandomysql.ExecuteNonQuery(); // Bye Bye 

                    TrocarPagina(0, $"O aluno \"{nome}\" foi removido dos dados."); // Volta o usuário para o menu.
                }
                else if (resposta == "0") // se o usuário colocou 0, então vai voltar para o menu
                {
                    TrocarPagina(0, $"Aluno {nome} não teve alterações.");
                }
                else // Caso coloque uma opção invalida, então vai voltar para esta pagina com nenhum mudança.
                {
                    Console.Clear();
                    Console.WriteLine("Campo inválido.\n");
                    AltRemAluno(id);
                }
            }
            else // Mensagem caso não encontre o id que o Usuário colocou. Também manda de volta para o menu.
            {
                Console.WriteLine("Não foi possivel encontrar o aluno com Id "+id+"\nDigite qualquer tecla para voltar ao menu.");
                Console.ReadKey();
                TrocarPagina(0);
            }
        }
    }
}

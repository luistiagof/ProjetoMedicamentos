using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjetoMedicamentos
{
    public class Lote
    {
        public int Id { get; set; }
        public int Qtde { get; set; }
        public DateTime Venc { get; set; }

        public Lote() { }

        public Lote(int id, int qtde, DateTime venc)
        {
            Id = id;
            Qtde = qtde;
            Venc = venc;
        }

        public override string ToString()
        {
            return $"{Id}-{Qtde}-{Venc:dd/MM/yyyy}";
        }
    }

    public class Medicamento
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Laboratorio { get; set; }
        public Queue<Lote> Lotes { get; set; } = new Queue<Lote>();

        public Medicamento() { }

        public Medicamento(int id, string nome, string laboratorio)
        {
            Id = id;
            Nome = nome;
            Laboratorio = laboratorio;
        }

        public int QtdeDisponivel()
        {
            return Lotes.Sum(l => l.Qtde);
        }

        public void Comprar(Lote lote)
        {
            Lotes.Enqueue(lote);
        }

        public bool Vender(int qtde)
        {
            if (QtdeDisponivel() < qtde)
            {
                return false;
            }

            int restanteParaVender = qtde;

            while (restanteParaVender > 0 && Lotes.Count > 0)
            {
                var loteAtual = Lotes.Peek();

                if (loteAtual.Qtde <= restanteParaVender)
                {
                    restanteParaVender -= loteAtual.Qtde;
                    Lotes.Dequeue();
                }
                else
                {
                    loteAtual.Qtde -= restanteParaVender;
                    restanteParaVender = 0;
                }
            }

            return true;
        }

        public override string ToString()
        {
            return $"{Id}-{Nome}-{Laboratorio}-{QtdeDisponivel()}";
        }

        public override bool Equals(object obj)
        {
            if (obj is Medicamento outro)
            {
                return this.Id == outro.Id;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class Medicamentos
    {
        private List<Medicamento> listaMedicamentos = new List<Medicamento>();

        public Medicamentos() { }

        public void Adicionar(Medicamento medicamento)
        {
            listaMedicamentos.Add(medicamento);
        }

        public bool Deletar(Medicamento medicamento)
        {
            var med = Pesquisar(medicamento);
            if (med.Id != 0 && med.QtdeDisponivel() == 0)
            {
                return listaMedicamentos.Remove(med);
            }
            return false;
        }

        public Medicamento Pesquisar(Medicamento medicamento)
        {
            var encontrado = listaMedicamentos.FirstOrDefault(m => m.Equals(medicamento));
            return encontrado ?? new Medicamento();
        }

        public List<Medicamento> ObterTodos()
        {
            return listaMedicamentos;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Medicamentos catalogo = new Medicamentos();
            int opcao = -1;

            while (opcao != 0)
            {
                Console.Clear();
                Console.WriteLine("========================================");
                Console.WriteLine("         PROJETO MEDICAMENTOS           ");
                Console.WriteLine("========================================");
                Console.WriteLine("0. Finalizar processo");
                Console.WriteLine("1. Cadastrar medicamento");
                Console.WriteLine("2. Consultar medicamento (sintético)");
                Console.WriteLine("3. Consultar medicamento (analítico)");
                Console.WriteLine("4. Comprar medicamento (cadastrar lote)");
                Console.WriteLine("5. Vender medicamento");
                Console.WriteLine("6. Listar medicamentos");
                Console.WriteLine("========================================");
                Console.Write("Escolha uma opção: ");

                if (!int.TryParse(Console.ReadLine(), out opcao))
                {
                    opcao = -1;
                }

                switch (opcao)
                {
                    case 0:
                        Console.WriteLine("\nFinalizando...");
                        break;
                    case 1:
                        CadastrarMedicamento(catalogo);
                        break;
                    case 2:
                        ConsultarSintetico(catalogo);
                        break;
                    case 3:
                        ConsultarAnalitico(catalogo);
                        break;
                    case 4:
                        ComprarMedicamento(catalogo);
                        break;
                    case 5:
                        VenderMedicamento(catalogo);
                        break;
                    case 6:
                        ListarMedicamentos(catalogo);
                        break;
                    default:
                        Console.WriteLine("\nOpção inválida!");
                        Console.ReadKey();
                        break;
                }
            }
        }

        static void CadastrarMedicamento(Medicamentos catalogo)
        {
            Console.Clear();
            Console.Write("ID do Medicamento: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                Console.Write("Nome: ");
                string nome = Console.ReadLine();
                Console.Write("Laboratório: ");
                string lab = Console.ReadLine();

                Medicamento med = new Medicamento(id, nome, lab);
                if (catalogo.Pesquisar(med).Id == 0)
                {
                    catalogo.Adicionar(med);
                    Console.WriteLine("\nCadastrado com sucesso!");
                }
                else
                {
                    Console.WriteLine("\nErro: Já existe um medicamento com este ID.");
                }
            }
            Console.ReadKey();
        }

        static void ConsultarSintetico(Medicamentos catalogo)
        {
            Console.Clear();
            Console.Write("ID do Medicamento a consultar: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                Medicamento busca = new Medicamento { Id = id };
                Medicamento med = catalogo.Pesquisar(busca);

                if (med.Id != 0)
                {
                    Console.WriteLine($"\nDados: {med}");
                }
                else
                {
                    Console.WriteLine("\nMedicamento não encontrado.");
                }
            }
            Console.ReadKey();
        }

        static void ConsultarAnalitico(Medicamentos catalogo)
        {
            Console.Clear();
            Console.Write("ID do Medicamento a consultar: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                Medicamento busca = new Medicamento { Id = id };
                Medicamento med = catalogo.Pesquisar(busca);

                if (med.Id != 0)
                {
                    Console.WriteLine($"\nDados: {med}");
                    Console.WriteLine("Lotes:");
                    foreach (var lote in med.Lotes)
                    {
                        Console.WriteLine($" - ID Lote: {lote.Id} | Qtde: {lote.Qtde} | Vencimento: {lote.Venc:dd/MM/yyyy}");
                    }
                }
                else
                {
                    Console.WriteLine("\nMedicamento não encontrado.");
                }
            }
            Console.ReadKey();
        }

        static void ComprarMedicamento(Medicamentos catalogo)
        {
            Console.Clear();
            Console.Write("ID do Medicamento: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                Medicamento busca = new Medicamento { Id = id };
                Medicamento med = catalogo.Pesquisar(busca);

                if (med.Id != 0)
                {
                    Console.Write("ID do Lote: ");
                    int idLote = int.Parse(Console.ReadLine() ?? "0");
                    Console.Write("Quantidade do Lote: ");
                    int qtde = int.Parse(Console.ReadLine() ?? "0");
                    Console.Write("Data de Vencimento (dd/mm/aaaa): ");
                    if (DateTime.TryParse(Console.ReadLine(), out DateTime venc))
                    {
                        Lote novoLote = new Lote(idLote, qtde, venc);
                        med.Comprar(novoLote);
                        Console.WriteLine("\nLote adicionado com sucesso!");
                    }
                    else
                    {
                        Console.WriteLine("\nData inválida.");
                    }
                }
                else
                {
                    Console.WriteLine("\nMedicamento não encontrado.");
                }
            }
            Console.ReadKey();
        }

        static void VenderMedicamento(Medicamentos catalogo)
        {
            Console.Clear();
            Console.Write("ID do Medicamento: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                Medicamento busca = new Medicamento { Id = id };
                Medicamento med = catalogo.Pesquisar(busca);

                if (med.Id != 0)
                {
                    Console.Write("Quantidade a vender: ");
                    if (int.TryParse(Console.ReadLine(), out int qtde))
                    {
                        bool sucesso = med.Vender(qtde);
                        if (sucesso)
                        {
                            Console.WriteLine("\nVenda realizada com sucesso!");
                        }
                        else
                        {
                            Console.WriteLine("\nErro: Quantidade indisponível em estoque.");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("\nMedicamento não encontrado.");
                }
            }
            Console.ReadKey();
        }

        static void ListarMedicamentos(Medicamentos catalogo)
        {
            Console.Clear();
            Console.WriteLine("--- LISTA DE MEDICAMENTOS ---");
            foreach (var med in catalogo.ObterTodos())
            {
                Console.WriteLine(med);
            }
            Console.ReadKey();
        }
    }
}

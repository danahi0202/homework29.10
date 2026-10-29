using System;
using System.Text;
using System.Threading;
using DatabaseLibrary;

class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        DatabaseService service = new DatabaseService();

        try
        {
            service.InitializeDatabase();
            Console.WriteLine("База данных готова.\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка подключения: " + ex.Message);
            return;
        }

        int productId = 0;

        Thread createThread = new Thread(() =>
        {
            try
            {
                Product product = new Product
                {
                    Name = "Keyboard",
                    Price = 1500
                };

                productId = service.Create(product);

                Console.WriteLine(
                    $"CREATE: товар добавлен. ID = {productId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("CREATE: ошибка — " + ex.Message);
            }
        });

        createThread.Start();
        createThread.Join();

        if (productId == 0)
        {
            Console.WriteLine("Работа остановлена: товар не создан.");
            return;
        }

        Thread readThread = new Thread(() =>
        {
            try
            {
                Product product = service.Read(productId);

                if (product != null)
                {
                    Console.WriteLine(
                        $"READ: ID={product.Id}, " +
                        $"Название={product.Name}, Цена={product.Price}");
                }
                else
                {
                    Console.WriteLine("READ: товар не найден.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("READ: ошибка — " + ex.Message);
            }
        });

        Thread updateThread = new Thread(() =>
        {
            try
            {
                Product product = new Product
                {
                    Id = productId,
                    Name = "Gaming Keyboard",
                    Price = 2000
                };

                bool updated = service.Update(product);

                Console.WriteLine(updated
                    ? "UPDATE: товар успешно обновлён."
                    : "UPDATE: товар не найден.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("UPDATE: ошибка — " + ex.Message);
            }
        });

        readThread.Start();
        updateThread.Start();

        readThread.Join();
        updateThread.Join();

        Thread deleteThread = new Thread(() =>
        {
            try
            {
                bool deleted = service.Delete(productId);

                Console.WriteLine(deleted
                    ? "DELETE: товар успешно удалён."
                    : "DELETE: товар не найден.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("DELETE: ошибка — " + ex.Message);
            }
        });

        deleteThread.Start();
        deleteThread.Join();

        Console.WriteLine("\nВсе потоки завершили работу.");
        Console.WriteLine("Нажмите Enter для выхода.");
        Console.ReadLine();
    }
}
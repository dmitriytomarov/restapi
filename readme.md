Проект является решением тестового задания по созданию Web Api для системы электронного магазина.</br>
Степень проработки - стартовая демонстрация возможностей.</br>
Если вы потенциальный работодатель, то наполненная БД для тестирования предоставляется по запросу.</br>
Telegram  https://t.me/Dmitriy_Tomarov
</br></br></br>
Техническое тестовое задание.

Цель:  продемонстрировать навыки работы с REST service, MS SQL SERVER, ООП, ASP.NET .NET Core API

Условия:  C#, .NET Core 3.1 и выше, Visual Studio 2019 и выше, MS SQL 2012 и выше </br>
(будет плюсом: БД должна подключаться в режиме LocalDB, БД должна быть нормализована, таблицы БД должны содержать необходимые данные для тестирования сервиса).

Задача:  создать REST API сервис заказной системы магазина. </br>
Сервис должен реализовывать следующие методы:

1) Метод получения списка клиентов.
2) Метод получения списка категорий товаров.
3) Метод получения списка товаров, с возможностью фильтрации по типу (категории) товара, по наличию на складе и сортировки по цене.
4) Метод получения списка заказов по конкретному клиенту за выбранный временной период, отсортированный по дате создания.
5) Метод формирования заказа с проверкой наличия требуемого количества товара на складе, а также уменьшение доступного количества товара на складе в БД в случае успешного создания заказа. В случае нехватки любого из запрошенных товаров на складе заказ не должен создаваться
6) Прочие методы, необходимые для корректной реализации любого клиентского приложения.

Методы сервиса должны корректно обрабатывать внештатные ситуации, исключительные ситуации должны быть обработаны и выданы корректно вызывающей стороне.
Для ускорения выдачи результатов для повторных вызовов, должно быть реализовано кэширование данных.
</br></br></br>
Будет плюсом:

Сервис должен быть задокументирован с помощью Swagger.</br>
Сервис должен реализовывать проверку подлинности на основе Basic Authentication. - не реализовано
</br></br></br>
Структура БД:</br></br>
Таблица с клиентами:</br>
•	ИД клиента</br>
•	ФИО клиента</br>
•	Телефон клиента</br>
</br>
Таблица с товарами:</br>
•	ИД товара</br>
•	ИД типа (категории) товара (список всех категорий выделить в отдельную таблицу, поле с foreign key)</br>
•	Наименование товара</br>
•	Цена товара</br>
•	Доступное количество товара на складе</br>
</br>
Таблица с заказами:</br>
•	ИД заказа</br>
•	ИД клиента</br>
•	Дата создания заказа</br>
</br>
Таблица с позициями заказов:</br>
•	ИД позиции товара в заказе</br>
•	ИД заказа</br>
•	ИД товара</br>
•	Цена</br>
•	Количество</br>
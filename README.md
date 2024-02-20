# Привіт Привіт, тут інстуркція як це використати

1. Імпорти вручну в notion 2 таблиці: [Інфобук для бази компаній - Sheet1.csv](https://github.com/pflofif/BestLvivCrmWorker/files/14213440/-.Sheet1.csv)
 та [База компаній  - updated_companies (3).csv.csv](https://github.com/pflofif/BestLvivCrmWorker/files/14213443/-.updated_companies.3.csv.csv).
2. В Базі компаній додай поле "посилання", зроби тип Relation на таблицю infobook:
     - ![image](https://github.com/pflofif/BestLvivCrmWorker/assets/93467423/b09043fd-08c5-44ef-a426-1f26663a7da3)
     - ![image](https://github.com/pflofif/BestLvivCrmWorker/assets/93467423/dd4c29c0-288f-47c3-98a9-067b485e8b2a)
     - ![image](https://github.com/pflofif/BestLvivCrmWorker/assets/93467423/044912d9-2007-499d-8c12-ccfb0e806278)

3. Ти повинен мати адмінку в середовищі де це створюєш. Зайди на сайт https://www.notion.so/my-integrations, там створи  нову інтеграцію, називай як хочеш ![image](https://github.com/pflofif/BestLvivCrmWorker/assets/93467423/a955358d-a6d8-4212-b1b1-de9c71837844)
4. Додай сторінки Infobook та Базу компаній до інтеграції:
   - ![image](https://github.com/pflofif/BestLvivCrmWorker/assets/93467423/27248636-df07-4354-b6b8-39ffa668ff82)

5. В коді на початку замість:
      - notionIntegrationToken встав токен інтеграції, він буде доступний зразу після створення
      - companiesDatabaseId - ІД сторінки з компаніями
      - informationBookDatabaseId - ІД сторінки інфобуку

6. Щоб взяти Ід сторінки, скопіюй посиланян неї
   
   ![image](https://github.com/pflofif/BestLvivCrmWorker/assets/93467423/d61c0d4b-ad4a-4160-8cce-935ac4d349b0)
    ![image](https://github.com/pflofif/BestLvivCrmWorker/assets/93467423/d08bbb50-b8a0-418a-b9b4-782f9d7f6589) - ІД це текст від '/' до '?'

8. Запусти програму!


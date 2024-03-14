# AviPrice

Проект разработанный в рамках Хакатона для кейса Авито "Платформа ценообразования". Данный проект представляет собой комплексную систему для управления ценообразованием. 

В рамки данного проекта входят:
- БД на MS SQL Server 
- Административная панель для аналитиков на ASP.NET
- API с алгоритмом для возврата цен на ASP.NET 

## Team

- [@SH4R1K](https://github.com/SH4R1K) - Team Leader👑, Backend
- [@Pluhenciya](https://github.com/Pluhenciya) - Frontend
- [@Morokenec](https://github.com/Morokenec) - Manager, DevOps
- [@4qiz](https://www.github.com/4qiz) - Frontend

## Stack

- .NET 8
- ASP.NET
- C#
- MS SQL Server 2022
- EFCore
- Docker

## Screenshots

![Страница основной матрицы](/github_res/сайт.png?raw=true ".")
![Скидочные матрицы](/github_res/скидочные.png?raw=true ".")
![Страница сбора стораджа](/github_res/сторадж.png?raw=true ".")
![Тестирование нагрузки](/github_res/постман.png?raw=true ".")
![Пример ответа](/github_res/родители.png?raw=true ".")

## Installation

1 Клонируйте репозиторий

2 Откройте решение в Visual Studio

3 Разверните БД, АПИ и Админ Панель на ваших ресурсах

4 Отредактируйте строки подключения и запросов

5 Запустите все проекты

## Docker - развертывание API

docker pull markelushakov/aviapi 

## Дополнительная информация
POST: /Storages/Update:
  Body byte[] storage - файл со стораджем, сериализованных с помощью Protobuf
  Обновляет сторадж в api
  Возвращает:
 -  200
 -  400

POST: /Locations/Update: 
  Body byte[] storage - файл с локациями, сериализованных с помощью Protobuf
  Обновляет локации в api
  Возвращает:
  - 200
  - 400

POST: /Categories/Update: - Обновление категорий для API
  Body byte[] storage - файл с категориями, сериализованных с помощью Protobuf
  Возвращает:
  - 200
  - 400

GET: /CellMatrixes - Получение цены из стораджа
  Query idLocation
  Query idCategory
  Query idUserSegment - необязательно
Возвращает:
200:
  - idMatrix - индекс матрицы, где найдена цена
  - price - цена
  - idLocation - локация, где найдена цена
  - idCategory - категория, где найдена цена
  - idUserSegment - сегмент пользователя
404:

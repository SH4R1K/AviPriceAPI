# AviPrice

Админ панель для управления ценами на услуги

## Team

- [@SH4R1K](https://github.com/SH4R1K) - Team Leader👑, DataBase Module
- [@Pluhenciya](https://github.com/Pluhenciya) - DataBase Module
- [@Morokenec](https://github.com/Morokenec) - Manager, DevOps
- [@4qiz](https://www.github.com/4qiz) - Design

## Stack

- .NET 8
- ASP.NET
- C#
- MS SQL Server 2022
- EFCore
- Docker

## Screenshots

![Main Screen](/github_res/mainscreen.png?raw=true ".")
![Second Screen Screen](/github_res/secondscreen.png?raw=true ".")

## Installation

1 Clone repository

2 Open solution in Visual Studio

3 Start two project

4 That's all

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

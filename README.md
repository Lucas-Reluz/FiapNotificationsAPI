# NotificationsAPI

API para gerenciar notificacoes dos usuarios da plataforma.

## O que faz

- Criar notificacoes automaticamente
- Buscar notificacoes de um usuario
- Marcar notificacoes como lidas
- Contar quantas notificacoes nao foram lidas

## Como funciona

A API escuta eventos do RabbitMQ. Quando um pedido e confirmado ou cancelado no PaymentsAPI, ela cria automaticamente uma notificacao para o usuario. As notificacoes ficam salvas no banco PostgreSQL.

## Como rodar

1. Ter Docker rodando com PostgreSQL e RabbitMQ
2. Entrar na pasta do projeto:
   ```
   cd NotificationsAPI/src/NotificationsAPI.Api
   ```
3. Rodar o comando:
   ```
   dotnet run
   ```
4. A API vai abrir em: http://localhost:5164

## O que precisa configurar

No arquivo appsettings.json tem:
- Conexao com banco PostgreSQL (NotificationsDb)
- Configuracao do RabbitMQ
- Chave secreta do JWT

## Endpoints principais

- GET /api/notifications/{id} - Buscar uma notificacao especifica
- GET /api/notifications/user/{userId} - Listar notificacoes de um usuario
- PATCH /api/notifications/{id}/read - Marcar notificacao como lida
- GET /api/health - Ver se a API esta funcionando

## Eventos que escuta

- OrderConfirmedEvent - Cria notificacao quando pedido e confirmado
- OrderCancelledEvent - Cria notificacao quando pedido e cancelado

## Tipos de notificacoes

- OrderConfirmed - Pedido confirmado com sucesso
- OrderCancelled - Pedido foi cancelado
- PaymentApproved - Pagamento foi aprovado
- PaymentFailed - Pagamento falhou

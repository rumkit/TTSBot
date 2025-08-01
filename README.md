# Telegram TorrServer Bot 
[![Build-Test-Publish Flow](https://github.com/rumkit/TTSBot/actions/workflows/build.yaml/badge.svg)](https://github.com/rumkit/TTSBot/actions/workflows/build.yaml)

## Description

A primitive Telegram bot for interacting with the TorrServer (https://github.com/YouROK/TorrServer). 
The bot has a single `/add` command which receives a magnet link or an http link to a page with the magnet link inside. 
The magnet link is added to the TorrServer using its API. 

The bot is build on top of the Minimal Telegram Bot: https://github.com/k-paul-acct/minimal-telegram-bot

## Docker Compose

Use the following example to quickly spin up the bot using the compose.yaml file. Keep in mind that TorrServer instance needs to be accessible for the bot app.
```yaml
name: TTS bot
services:
  ttsbot:
    image: ghcr.io/rumkit/telegram-torrserver-bot:latest
    build:
      context: .
      dockerfile: TTSbot/Dockerfile
    environment:
      BotToken: #telegram bot token
      TorrServer__User: #torrserver user
      TorrServer__Password: #torrserver password
      TorrServer__Url: #torrserver url e.g. http://127.0.0.1:8080
      Telegram__AllowedChatId: #OPTIONAL if provided, bot will process any commands only from the specified chat
```

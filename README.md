# HoNGuideParser

A small console application that parses the guides from www.heroesofnewerth.com/guides and creates HoN compatible guides.

## Downloads

* [Full collection of **WhatYouGot** guides](https://drive.google.com/drive/folders/1ji8TzG33RCGullyxeHnZOZOxSOeRVfHO)

## How to install

Download the files into `/game/guides`.

## Development

Guides can be found in `/game/resources0.s2z/Guides`.
Each guide should be named `hero_%hero_name%_guide.txt`.

The guide file consists of the following parts:

1. Guide creation date in format "MM/dd/yy HH:mm:sstt"
2. Guide creator name (probably)
3. Hero_%hero_name%
4. Name of the guide
5. Hero name
6. Guide version (probably)
7. List of the recommended items 
8. List of the recommended skill order

Example:

1. 06/23/22 16:46:36PM
2. Nome
3. Hero_Zephyr
4. Zephyr Guide
5. Zephyr
6. 1.00
7. Item_GoldenApple|Reborn_Item_IronBuckler|...
8. Ability_Zephyr2|Ability_Zephyr1|...

Be careful with all the separators and formatting, because if you missed any special symbol, the guide will not load.

If you want to create your guide, create a folder `guides` in `/game/` and copy an existing guide to it. Next, you can modify it however you want.

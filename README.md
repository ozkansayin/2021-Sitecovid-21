![Hackathon Logo](docs/images/hackathon.png?raw=true "Hackathon Logo")
# Sitecore Hackathon 2021

- MUST READ: **[Submission requirements](SUBMISSION_REQUIREMENTS.md)**
- [Entry form template](ENTRYFORM.md)
- [Starter kit instructions](STARTERKIT_INSTRUCTIONS.md)

# Hackathon Submission Entry form

## Team name
Sitecovid-21

## Category
The best enhancement to the Sitecore Admin (XP) for Content Editors & Marketers

## Description
This module, called Smart Navigation, produces navigation suggestions, which aim to help reduce the number of steps content editors need to take to work on different items. It does this by watching the actions taken on the items, and guessing what the next item would be to be worked on. These suggestions are rendered on the Content tab, easy to notice & fast to use. Clicking on the generated links selects the corresponding item in the content tree.


![FieldSection](docs/images/SmartNavigationFieldSection.png?raw=true "FieldSection")

## Video link

[Youtube](https://www.youtube.com/watch?v=gVZJE0fZlr8)

## Installation instructions

You can use the module either by installing it as a [Sitecore Package](scpackages/SmartNavigation-1.2.zip), or using the source code.

### Via the Sitecore package

Installing the Smart Navigation via the [Sitecore Package](scpackages/SmartNavigation-1.2.zip) is pretty straightforward. Just install the package [SmartNavigation-1.2.zip](scpackages/SmartNavigation-1.2.zip) as always, and it should work.

If you get an error while installing the package, like "Access denied", you might be working in a docker environment with an application pool that does not have enough permissions. We were able to bypass this by adding the following lines to our dockerfile for cm ([\docker\build\cm\dockerfile](docker/build/cm/Dockerfile#L32))

`# Grant full access to wwwroot to prevent package installation issues`<br />
`RUN icacls 'C:\inetpub\wwwroot' /grant 'IIS_IUSRS:(F)' /t`

### Via the source code

* Get your docker environment ready by running the .\Start-Hackathon.ps1
* Build the Feature.SmartNavigation feature (in case Start-Hackathon.ps1 somehow failed to build & publish)
* Done.

### Configuration
There are no mandatory configurations to be done. Just in case you want to change the database file path that is used by the suggestion engine (via LiteDB), you can update the setting with key "SmartNavigation.DbFilePath" at path "\App_Config\Include\Feature\Feature.SmartNavigation.Settings.config", which defaults to "$(dataFolder)/smartNavigation.db"

## Usage instructions

There are no specific things the users need to do to get the package working. The package will start tracking item changes right after the installation. After the database is generated for the items, a new field section will become visible in the Content tab of the Content Editor.

![FieldSection](docs/images/SmartNavigationFieldSection.png?raw=true "FieldSection")

Let's see what each section offers. 
### Next Item Suggestions
First part with title "You might want to work on these next" contains a list of items that you are likely to use next. 

![NextNavigation](docs/images/NextNavigation.png?raw=true "NextNavigation")

Clicking on any of these links will auto-select the corresponding item in the content tree, unfolding any ascendant in the content tree as needed.

![ClickNavigation](docs/images/ClickNavigation.png?raw=true "ClickNavigation")

### Previous Item Suggestions
Here the module lists the items from which the current item is likely to be the next item to be worked on. It can be useful if the content editors need to go back.

![PrevNavigation](docs/images/PrevNavigation.png?raw=true "PrevNavigation")
### Last worked item
Here the module simply lists the last item you worked on, in case you need to go back to.

![lastItemNavigation](docs/images/lastItemNavigation.png?raw=true "lastItemNavigation")
## Comments
We have prepared an Ad for you, enjoy

---

## ~Ad Notice~

## Smart Navigation by Sitecovid-21

Are you a content editor getting lost when trying to find the item you need to work on? Are you a developer tired going back and forth to create the demo content in the content tree? 

Have no fear, Smart Navigation (*patent not even pending*) is here!

Smart Navigation is a tool designed to predict the next item(s) (or the previous ones) you might want to work on, based on the history of the item change. It watches the CRUD operations on Sitecore items, trains the rule-based expert system (woah!) and comes up with item navigation suggestions for you to follow. All in the comfort of your beloved Content Editor!

## How it works

You do you! People (or *Content Editors*, if you fancy titles) just keep working business as usual. In the meantime, we catch item events in the background, and feed our expert system's rule database. After we have the data, you will see a new field section on top of the content tab called Smart Navigation right below the infamous Quick Info section.

This section consists of three possible sets of information.
### Future items
Here we list our predictions as to which items you might want to travel. Do you frequently create setting items in a folder after creating a dictionary item? You will see the folder here. Do you go to your *Parameters Template* folder after creating a rendering, you will see it here. Just click on it and woosh! You are there!

![ClickNavigation](docs/images/ClickNavigation.png?raw=true "ClickNavigation")
### Past items
Here we list the items from which we predict you are likely to go to the current item, as we do not think all the *ex*es are bad. Click on an item and voila! You are back to the fu... past(?).

![PrevNavigation](docs/images/PrevNavigation.png?raw=true "PrevNavigation")
### Last Item
Here we show the last item you worked on. Nothing fancy, gets the job done.

![lastItemNavigation](docs/images/lastItemNavigation.png?raw=true "lastItemNavigation")
> That's All Folks
# Gameboard Designer Guide

This documentation assumes that you have authenticated to the Gameboard app and have been granted the Game Designer role. Reminder: a *game* contains *boards*. Boards contain *challenges*.

In the Gameboard, click the **Game Designer** icon.

In the Game Designer screen, you can:

- Click the **Games** icon to list all of the available games
- Click the **New** " + " icon to view the new GAME and BOARD panels where you create a new game and boards
- Sort challenges in the results by game **Id** (the game's unique identifier) or **Name**

### Game Designer icons:

- **Edit Game:** Launches the Manage Game screen where your game is created and updated.
- **Save Game:** After committing, click Save to save game updates.
- **Add Board:** Adds a board to the game.
- **Add Category:** Add a category to the board. An example of a category in a board could be the name of a competition round, like "Round 1".
- **Edit Board:** Launches the Manage Board screen where your board is created and updated.
- **Remove Board:** Deletes the board from the game.
- **Move Up / Move Down:** Can adjust the order of the game boards up or down if there are more than one.

## Creating a new game

In the Game Designer, click the **New** " + " icon to launch the GAME and BOARDS panels.

Click **Edit Game**. In the Manage Game screen, complete the following fields:

- **Id:** The unique identifier for the game.
- **Name:** The name of the game which is displayed in the gameboard COMPETITION panel.
- **Min Team Size:** The minimum number of participants who are required to comprise a team.
- **Max Team Size:** The maximum number of participants who are allowed comprise a team.
- **Max Concurrent Gamespaces:** The maximum number of concurrent "gamespaces" allowed. A gamespace is the virtual environment that participants use to compete in a challenge. The default value is 0; the value that you enter here is inherited by a newly created board. For example, if you set this value to 5 in the *game*, any *board* created will inherit the 5 concurrent gamespace setting. 
- **Date** and **time** enrollment ends: The date and the time that enrollment in your game ends.
- **Date** and **time** the game starts: The date and the time that your game begins.
- **Date** and **time** the game stops: The date and the time that your game ends.

After clicking **Commit**, the GAME becomes visible in the panel. Click the green **Save** icon. Next, boards have to be added to your game.

## Adding a new board

In the BOARDS panel, click the **Add Board** icon. Then, click **Add Category**.

Select the newly added category. In the Manage Category screen, complete the following fields:

- **Name** of this category: The overall name for the new category. For example, you could create categories that match up with NICE Cybersecurity Framework Work Roles. A category is just a way to group challenges that have some subject matter in common.
- **Point Value:** The number of points you want to assign to an associated challenge within the category.
- **Associated challenge:** A challenge associated with the category. Challenges found here are challenges that have been added by users who have the Challenge Developer role. For help with developing challenges for the gameboard, see the [Challenge Developer Guide](https://wiki-int.sei.cmu.edu/confluence/display/CWD/Challenge+Developer+Guide).
- **Disable question:** Select Disabled to disable a challenge in the category. Upon disabling the challenge, the point value is visible in the board but will appear "grayed out". Participants will *not* be able to view or launch that challenge.

Click the blue "+" icon to add more challenges. After clicking **Commit**, the category and its associated challenges are visible in the BOARDS panel. Click the green **Save** icon.

Add as many categories to your board as you need for your competition. After you have added categories, use the **<** and **>** icons to move categories to the left or right in the board.

Next, boards have to be edited.

### Editing a board

In the BOARDS panel, click **Edit Board**. In the Manage Board screen, complete the following fields.

- **Name** of this board: The name for the new board.
- **Type:** Either **Trivia** (like the game show *Jeopardy!*) or **Map** (where challenges show up as hot spots on a map interface).
- **Max Submissions:** The maximum number of solutions a participant can send to the grading server per challenge--whether that submission is correct, incorrect, or blank.
- **Max Minutes:** The maximum number of time in minutes that participants have to complete as many challenges as they can.
- **Certificate Threshold:** The minimum number of points a participant must earn in order to print a certificate for the board.
- **Max Concurrent Gamespaces:** The maximum number of concurrent "gamespaces" allowed. A gamespace is the virtual environment that participants use to compete in a challenge. The default value is 0; the value here is inherited from what was entered in the **Max Concurrent Gamespaces** setting in the Manage Game screen. If the value is 0, you can have as many active gamespaces as there are challenges on a board.
- **Start text:** This is the text a participant will see when they select a board in the competition. Items to include here might be time constraints, reminders of competition rules, and other information pertinent to the competition.
- **Date** and **time** the board starts: The date and the time that your board begins.
- **Date** and **time** the board stops: The date and the time that your board ends.
- **Badges that can be earned for board participation:** Badges that appear here are determined by what is in the **settings.json** file. Select a badge (or badges) that can earned by competition participants.
- **Badges required to start this board:** Select a badge (or badges) that participants are required to earn prior to starting a board. Participants who attempt to start a board who have not earned the badge to start it will receive this message: *Team does not meet board prerequisites.*

Badges are not awarded automatically upon completion of a board. Badges are awarded to participants in the Leaderboard using the **Manage Badges** feature. To use the Manage Badges feature to assign badges, you must have *moderator-*level permission.

#### Board options

The board options below can be enabled/disabled as needed for your board.

- **Practice**: Check to exclude practice boards from reporting.
- **Preview Allowed:** Check to allow participants to view the board prior to starting.
- **Allow Reset:** Check to allow participants to restart their board and attempt challenges again.
- **Show Titles:** Check to display challenge titles on the board.
- **Allow Shared Workspaces:** Check to enable shared workspaces across challenges. See the section "Understanding Shared Resources" below.

Icons representing each option appear in the board panel when selected.

Click **Commit**.

> Remember to click the green **Save** icon to save your changes. Clicking **Commit** closes the window and holds your changes, but they are not *saved* until you click **Save**.

This documentation assumes that you have authenticated to the Gameboard app and have been granted the Game Designer role. Reminder: a *game* contains *boards*. Boards contain *challenges*.

In the Gameboard, click the **Game Designer** icon.

In the Game Designer screen, you can:

- Click the **Games** icon to list all of the available games
- Click the **New** " + " icon to view the new GAME and BOARD panels where you create a new game and boards
- Sort challenges in the results by game **Id** (the game's unique identifier) or **Name**

#### Understanding Shared Workspaces

Checking the **Allow Shared Workspaces** is a board-level setting that allows challenge participants from the same team to share a single workspace across multiple challenges. Only a single workspace is supported for a board with this option enabled. For example: if 10 challenges are assigned to a board and five of them use a workspace, all of those challenges must have the same workspace Id. However, you can still have challenges on that board that don't require a workspace. 

Enabling this feature also effects the lifetime of an associated workspace. On boards without shared workspaces each challenge can have an associated workspace. The workspace is removed and its resources are reclaimed immediately after a participant completes a challenge successfully or by exhausting all available submissions for a challenge. When shared workspaces are enabled a single instance of the workspace is started when any of the challenges with the specified workspace Id is started. This instance of the workspace will continue to exist until all related challenges on the board are completed (successfully or unsuccessfully) or until the timer for the board runs out.

During this time a single instance of the workspace exists and all team members launching challenges with this workspace Id are sent to the same workspace environment. This means that changes made by a single user or another team member can effect everyone in that workspace – and their potential outcomes.

There are also certain features where a workspace can be deleted and/or reset. This effects all current users of the workspace and can significantly impact the user or their team members' progress. Examples where a shared workspace can be deleted include: resetting a challenge, resetting a board, or deleting and restarting an active challenge's workspace. When a participant attempts to make one of these resets they are warned that the board is using shared workspaces and the change may impact other team members. 

### Game Designer icons:

- **Edit Game:** Launches the Manage Game screen where your game is created and updated.
- **Save Game:** After committing, click Save to save game updates.
- **Add Board:** Adds a board to the game.
- **Add Category:** Add a category to the board. An example of a category in a board could be the name of a competition round, like "Round 1".
- **Edit Board:** Launches the Manage Board screen where your board is created and updated.
- **Remove Board:** Deletes the board from the game.

## Creating a new game

In the Game Designer, click the **New** " + " icon to launch the GAME and BOARDS panels.

Click **Edit Game**. In the Manage Game screen, complete the following fields:

- **Id:** The unique identifier for the game.
- **Name:** The name of the game which is displayed in the gameboard COMPETITION panel.
- **Min Team Size:** The minimum number of participants who are required to comprise a team.
- **Max Team Size:** The maximum number of participants who are allowed comprise a team.
- **Max Concurrent Gamespace:** The maximum number of concurrent "gamespaces" allowed. A gamespace is the virtual environment that participants use to compete in a challenge.
- **Date** and **time** enrollment ends: The date and the time that enrollment in your game ends.
- **Date** and **time** the game starts: The date and the time that in your game begins.
- **Date** and **time** the game stops: The date and the time that in your game ends.

After clicking **Commit**, the GAME becomes visible in the panel. Click the green **Save** icon. Next, boards have to be added to your game.

## Adding a new board

In the BOARDS panel, click the **Add Board** icon. Then, click **Add Category**.

Select the newly added board. In the Manage Category screen, complete the following fields:

- **Name** of this category:
- **Point Value:**
- **Associated challenge:**

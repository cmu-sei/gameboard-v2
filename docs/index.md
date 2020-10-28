# Gameboard Roles Guide

The gameboard documentation is organized by *roles*. There are five: participant, observer, moderator, game designer, and challenge designer.

## Participant, observer, and moderator

A *participant* is someone who is competing or participating in the game. A participant won't see all of the reports that are available, for example. Most people will be participants.

An *observer* is the next permission level up. An observer can:

- View team names, both real and anonymized
- View earned badges 
- View team organization
- View team create time
- View team status
- Export team data to a CSV file

A *moderator* can do everything an observer can do on the Teams page, plus: 

- Edit team names
- Email teams
- View the members on a team
- Set override minutes
- Assign badges
- Disable and enable teams
- Send broadcast system messages
- Grant permissions (observer, challenge developer, game designer)
- Export the Completion Report, Leaderboard, challenge feedback data, and team activity

Participant, observer, and moderator information can be found in the [Gameboard Participant Guide](./gb-participant-guide).

## Game Designer

A *game designer* builds a new game space; that is, the framework that contains the game challenges. A game contains *boards*. Boards contain *challenges*. A game designer can:

- Create and edit games
- Manage enrollment dates, team sizes, and limit concurrent game spaces
- Create and edit multiple boards per game
- Set board options, including: trivia vs map, board layouts, start and and times, allow resets, preview, practice mode, etc.
- Arrange challenges on a board
- Specify category and point values
- Assign background images to boards

Game designer information can be found in the [Game Designer Guide](./gb-designer-guide).

## Challenge Developer

A *challenge developer* creates the challenges that populate the game boards. A challenge developer can:

- Create and edit game challenges
- Upload and download YAML challenge files 
- Add workspaces
- Manage flags, tokens, and grading

Challenge developer information can be found in the [Challenge Developer Guide](./gb-challenge-dev-guide).

## Roles Matrix

| Role                | View Teams | Manage Teams | Email Teams | View Members | Export Teams | System Messages | View Users | Manage Users | Manage Games | Manage Challenges |
| :------------------ | :--------: | :----------: | :---------: | :----------: | :----------: | :-------------: | :--------: | :----------: | :----------: | :---------------: |
| Moderator           |     ✓      |      ✓       |      ✓      |      ✓       |      ✓       |        ✓        |     ✓      |      ✓       |              |                   |
| Observer            |     ✓      |              |             |              |      ✓       |                 |            |              |              |                   |
| Game Designer       |            |              |             |              |              |                 |            |              |      ✓       |         ✓         |
| Challenge Developer |            |              |             |              |              |                 |            |              |              |         ✓         |

## License

Copyright 2020 Carnegie Mellon University. See the [license](https://github.com/cmu-sei/gameboard-ui/blob/master/License.md) file for details.

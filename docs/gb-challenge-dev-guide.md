# Challenge Developer Guide

This documentation assumes that you have authenticated to the Gameboard app and have been granted the Challenge Developer role.

In the Gameboard, click the challenge developer **Flag** icon.

In the Challenge Developer screen, you can:

- click the **Challenges** icon to list all of the challenges
- click the **New** " + " icon to launch the Manage Challenge screen where you create a new challenge
- Search for a challenge using the **All**, **Multi Part**, and **Multi Stage** radio buttons and the **search...** field
- Sort challenges in the results by **Slug** (the challenge's unique identifier) or **Title**

#### Challenge Icons

- **Manage Challenge:** Launches the Manage Challenge screen where your challenge is created and updated; ultimately, the options here create a YAML file.
- **Upload YAML:** Upload an existing challenge YAML file to this gameboard. YAML could have been created in another instance of a gameboard or created by hand.
- **Download YAML:** Download challenge YAML specifications here to edit elsewhere or upload for use in another instance of a gameboard.
- **Save:** After committing, click **Save** to save challenge updates.
- **Delete:** Deletes the challenge from the gameboard.

## Creating a new challenge

In the Challenge Developer screen, click the **New** " + " icon to launch the Manage Challenge screen. Complete the following fields:

- **Slug:** The unique identifier for the challenge (*required*).
- **Title:** The title of the challenge which is displayed in the gameboard (*required*).
- **Tags:** Tags that describe the challenge. Multiple tags should be separated by a comma. Examples of tags might be `forensics` or `cyber analysis` as they pertain to the type of challenge.
- **Multi Stage:** Indicate whether the challenge must be solved in sequential order.
- **Document:** Attach a downloadable .pdf. The document identified here is visible on the actual challenge within the gameboard.
- **Suggest total points:** How many points you think the challenge is worth.
- **Difficulty:** On a scale of 1 to 5, with 5 being the most difficult, rate the challenge.
- **Flag Style:** A *token* or *text* that must be submitted for a grade.
- **Short description:** A short description of the challenge. Markdown syntax can be used to format the description. The short description is displayed in the gameboard before a participant starts the challenge. This way, a participant can see what the challenge is about without starting the timer (*required*).
- **Comprehensive text:** The instructions or "guide" for the challenge. Markdown syntax can be used to format the comprehensive text. The comprehensive text is what the participant sees after the challenge has been started. Include submission format and and system and tool credentials here (*required*). You can add an image here by uploading the image file to the **status/img** folder on the QNAP Server `bmqnap.ad.sei.cmu.edu`, and then insert the image into your challenge text: `![Image Title](img/your-image.png)` .

After clicking **Commit**, the **WORKSPACE** area becomes visible. Then, click the green **Save** icon.

### Workspace

In the **WORKSPACE** area, click the the **New** " + " icon to add a workspace.

- **Workspace ID:** This is the workspace ID in TopoMojo. Challenges can be completed off-line; however, a TopoMojo workspace is always provided.
- **Customize Templates:** Check Customize Templates if you want to customize workspace templates. The `_template.json` file has default values; when checked, any custom setting keys that have been added to the template will be applied. You can use these custom settings to assign `guestinfo` settings via VMware Tools.
- **Templates:**
- **ISO:** ISO's can be found on QNAP Server `bmqnap.ad.sei.cmu.edu`; name the ISO here.
- **ISO Target:** If you want an ISO attached to a specific VM, then name it here.
- **Host Affinity:** Check Host Affinity if you want all VMs in the workspace to come up on the same ESX Server.
- **Append Markdown:** Check Append Markdown if you want the Lab Document (Markdown) from TopoMojo appended.

#### Network

- **New IP:**
- **Hosts:**
- **Dnsmasq:**

#### Virtual Machines

- **Name:** If the VM has a special case (needs multiple instances or does not need the ISO mounted), then name it here.
- **Replicas:** Enter the number of instances here. -1 can be used to generate an instance of the VM for each member of the team
- **Skip ISO:** Check Skip ISO to skip mounting the ISO on the VM.

> Reminder! After clicking **Commit**, click the green **Save** icon.

## Managing Flags

- **Match**:

  - **Match:** Matching one thing and one thing *only*. `token: a-literal-token`

  - **MatchAll:** Match all items in a list; the order does not matter. `token: token1|token2|token3`
  - **MatchAny:** Match any item within a list. `token: token1|token2|token3`
  - **MatchOutput:** A grading script runs on a virtual machine that compares its output to the output specified.
  - **MatchAlphaNumeric:** All characters that are not alphanumeric are dropped from the string. For example the IP address `10.0.0.100` is read as `10001000`. `token: 001122n334455w`

- **Value**: The default value is `1` which is 100 percent of the points allocated to the challenge.

- **ISO Instruction:**

- **ISO Restricted:** Only attached to VM; not downloadable.

- **Text:** Text specific to that flag variant gets appended to the *Challenge Guide*; what the participants see.

#### Tokens

Define the answers here. Questions are written in the *text to append to challenge text for this flag*.

- **Label**: Create a label here so participants can identify which answer to enter.

#### Generation

- **Generate Output Text:** The file which contains custom text. This text will be appended to the regular challenge text. 
- **Generate Output Flag:** The file which contains the generated flag. For single-part challenges, only the flag needs to be present. In multi-part challenges, this file needs to contain yaml-formatted text for each token.
- **Generate Output File List:** The file which contains a list of file names to include in an ISO file. Each of the named files will be wrapped up as a single ISO and attached to the challenge VMs.
- **Generate Command:** Any command the challenge developer wants generated "on the fly". You may also assign values to custom setting keys applied to a template. For example: `sed -i -d "s/##token##/abcdefg/" /dst/_templates.json` will substitute the value `abcdefg` as the token guest variable. This command will be run inside a Docker container. 
- **Generate Image:** The Docker image to run the Generate Command in.

#### Grading

- **Grade Input Flag:**
- **Grade Input Data:**
- **Grade Input File:**
- **Grade Output File:**
- **Grade Command:** The command to run when grading the challenge. This command will be run inside a Docker Container. 
- **Grade Image:** The Docker Image to run the Grade Command in.
- **Grade Command Timeout:** How long to allow the Grade Command to run before it times out (fails).

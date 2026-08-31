# Dist-O-Meter



Dist-O-Meter is a distance measurement application that calculates the position and distance of objects based on a known baseline and two measured angles.



The project consists of:



- A Python FastAPI backend for calculations and plotting

- An ASP.NET Core MVC frontend

- Automated PowerShell scripts for setup and startup



---



## Quick Start



The easiest way to run Dist-O-Meter is with the included PowerShell scripts.



### 1. Clone the repository



```powershell

git clone https://github.com/KristofferSolum/Dist-O-Meter.git

cd dist-o-meter

```



### 2. Run setup



```powershell

.\\scripts\\setup.ps1

```



The setup script automatically:



- Downloads the required Python environment if needed

- Creates the Python virtual environment

- Installs the backend dependencies

- Downloads .NET 8 SDK if needed

- Builds the ASP.NET Core application



You do \*\*not\*\* need to install Visual Studio, Python, or .NET manually.



### 3. Start Dist-O-Meter



```powershell

.\\scripts\\run.ps1

```



This starts both:



- FastAPI backend

- ASP.NET Core web application



The Dist-O-Meter website will automatically open in your browser.



Default address:



```text

http://127.0.0.1:5000

```



---



## Requirements



For the automatic setup, you only need:



- Windows

- PowerShell

- Git

- Internet connection during the first setup



All other required development/runtime tools are downloaded automatically by `setup.ps1`.



---



## How It Works



Dist-O-Meter uses a known baseline between two reference points, `R` and `Q`.



The point `P` is always located exactly in the middle of the baseline.



```text

R -------- P -------- Q

```



For each object, the user enters:



- Object name

- Angle measured from R

- Angle measured from Q



The backend triangulates the object position and calculates:



- Distance from P to each object

- Distance between every pair of objects

- Object coordinates

- A coordinate plot of the measurement



All distances are measured in centimeters.



---



## Using the Application



1. Enter the baseline length in centimeters.

2. Enter a name and the two measured angles for the first object.

3. Use **+ Add object** to add additional objects.

4. Press **Calculate**.

5. The results page displays:

&#x20;  - Distances from P to each object

&#x20;  - Distances between objects

&#x20;  - Object coordinates

&#x20;  - Measurement plot



---



## Development



If the project has already been set up, it is not necessary to run the setup script every time.



Simply run:



```powershell

.\\scripts\\run.ps1

```



If dependencies or the development environment change, run:



```powershell

.\\scripts\\setup.ps1

```



again.



The setup script is designed to be safe to run multiple times.


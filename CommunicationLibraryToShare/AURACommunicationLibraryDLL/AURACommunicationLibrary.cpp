#include <windows.h>
#include "stdafx.h"
#include <string>
#include < functional >
#include < utility >
#include < string >
#include < cstdlib > //std::system
#include < iostream >
#include < windows.h >
#include <tchar.h>
#include <iomanip>
#include <sstream>


using namespace std;

extern "C" { __declspec(dllexport) void InitialiseComm(); }
extern "C" { __declspec(dllexport) void CommunicateData(); }
extern "C" { __declspec(dllexport) double GetData(int robotNumber, int direction); }
extern "C" { __declspec(dllexport) void  SetRobot(int robotNumber, float leftVelocity, float rightVelocity); }

LPTSTR szMemoryName = _T("Local\\mysharedmem");
LPTSTR szEventName = _T("Local\\mysharedevent");
HANDLE hMem, hEvent;
void * memory;

double mainData[11][3] = { 0 };

float robotVelocities[5][2] = { 0 };

void  SetRobot(int robotNumber, float linearVelocity, float angularVelocity) {
	robotVelocities[robotNumber][0] = linearVelocity;
	robotVelocities[robotNumber][1] = angularVelocity;
}

void InitialiseComm()
{
	hEvent = CreateEvent(NULL, FALSE, FALSE, szEventName);
	if ((hMem = OpenFileMapping(FILE_MAP_READ | FILE_MAP_WRITE, FALSE, szMemoryName)) == NULL) {
		//cannot open shared memory, so we are the first process: create it
		hMem = CreateFileMapping(INVALID_HANDLE_VALUE, NULL, PAGE_READWRITE, 0, 1024, szMemoryName);
		memory = MapViewOfFile(hMem, FILE_MAP_READ | FILE_MAP_WRITE, 0, 0, 0);
	}
}

double GetData(int itemIndex, int dataIndex) {
	return mainData[itemIndex][dataIndex];
}

void CommunicateData()
{
	string incomingString = "";

	//Wait for the robot soccer program to send us some data (with a really long timeout)
	switch (WaitForSingleObject(hEvent, 500000000))
	{
		case WAIT_OBJECT_0:
			//Data has been received

			//This is where we are going to put the data we get from RS programsa
			double data[32] = {
				0
			};

			//The data given to us is a string
			std::string string = (LPCSTR)memory;
			int currentEnd = 0;
			int i = 0;
			int j = 0;

			//Split and parse the string
			for (j = 0; j < 31; j++) {
				std::string string1;
				std::string string2;
				string1 = string;
				string2 = string;
				data[j] = atof(string1.substr(i, 5).c_str());
				i = i + 5;
			}

			//Set the robot data
			mainData[0][2] = (data[0] - 5);
			mainData[0][0] = (data[1] - 5);
			mainData[0][1] = (data[2] - 5);

			mainData[1][2] = (data[3] - 5);
			mainData[1][0] = (data[4] - 5);
			mainData[1][1] = (data[5] - 5);

			mainData[2][2] = (data[6] - 5);
			mainData[2][0] = (data[7] - 5);
			mainData[2][1] = (data[8] - 5);

			mainData[3][2] = (data[9] - 5);
			mainData[3][0] = (data[10] - 5);
			mainData[3][1] = (data[11] - 5);

			mainData[4][2] = (data[12] - 5);
			mainData[4][0] = (data[13] - 5);
			mainData[4][1] = (data[14] - 5);

			//Get the opponent robot data
			mainData[5][0] = (data[17] - 5);
			mainData[5][1] = (data[18] - 5);

			mainData[6][0] = (data[19] - 5);
			mainData[6][1] = (data[20] - 5);

			mainData[7][0] = (data[21] - 5);
			mainData[7][1] = (data[22] - 5);

			mainData[8][0] = (data[23] - 5);
			mainData[8][1] = (data[24] - 5);

			mainData[9][0] = (data[25] - 5);
			mainData[9][1] = (data[26] - 5);

			//Set the ball data
			mainData[10][0] = data[15];
			mainData[10][1] = data[16];

			break;
	}

	//Start of segment to send velocity values back to RobotSoccerProgram
	std::string message = "";
	int j = 0;
	LPTSTR szMemoryName1 = _T("Local\\mysharedmem1");
	LPTSTR szEventName1 = _T("Local\\mysharedevent1");
	HANDLE hMem1, hEvent1;
	void * memory1;

	hEvent1 = CreateEvent(NULL, FALSE, FALSE, szEventName1);
	hMem1 = OpenFileMapping(FILE_MAP_READ | FILE_MAP_WRITE, FALSE, szMemoryName1);
	std:ostringstream strs;
	std::string str;

	double hold;
	//Create a string with all the data we need
	for (int i = 0; i < 5; i++) {
		message = "";

		//We add 500 to make it unsigned, not sure why its done this way, pretty bad lol
		hold = (double)(robotVelocities[i][0] + 500);
		strs << std::fixed << std::setprecision(3) << hold << "";
		hold = (double)(robotVelocities[i][1] + 500);
		strs << std::fixed << std::setprecision(3) << hold << "";
		str = strs.str();

		message = message + str;
	}

	//Yeah this message footer is a thing :(
	message = message + "ENDDDDD";
	
	memory1 = MapViewOfFile(hMem1, FILE_MAP_READ | FILE_MAP_WRITE, 0, 0, 0);
	CopyMemory(memory1, message.c_str(), message.length() + 1);

	//Notify robot soccer program
	SetEvent(hEvent1);

	//Close handles
	CloseHandle(hMem1);
	CloseHandle(hEvent1);
}




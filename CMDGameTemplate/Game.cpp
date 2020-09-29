#include "Game.h"
#include "GameMode.h"

bool Game::Initialize()
{
	std::ios::sync_with_stdio(false);
	tickCount = clock();

	gameMode = new GameMode();
	return true;
}

void Game::Loop()
{
	while (isRunning) {
		ProcessInput();
		UpdateGame();
		GenerateOutput();
	}
}

void Game::Shutdown()
{
	delete gameMode;
}

void Game::ProcessInput()
{
	//处理输入
	gameMode->ProcessInput();
}

void Game::UpdateGame()
{
	//锁定60帧率
	while (clock() - tickCount < 16);
	currentTick = clock();
	//计算时间步长
	float deltaTime = static_cast<float>((currentTick - tickCount)) / CLOCKS_PER_SEC;
	tickCount = currentTick;
	gameMode->UpdateGame(deltaTime);
}

void Game::GenerateOutput() {
	//输出图像
	system("cls");//清屏
	std::string str = "";
	gameMode->GenerateOutput(str);
	std::cout << str;
}
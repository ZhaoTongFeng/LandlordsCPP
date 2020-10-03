#include "BaseGameMode.h"

BaseGameMode::BaseGameMode():
	mSession(GameSession::PREPARE),
	mCardsHandle(nullptr)
{
}

BaseGameMode::~BaseGameMode()
{
	for (int i = 0; i < players.size(); i++) {
		delete players[i];
	}
	delete mCardsHandle;
}

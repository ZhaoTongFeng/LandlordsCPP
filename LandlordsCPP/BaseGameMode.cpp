#include "BaseGameMode.h"

BaseGameMode::BaseGameMode():
	mSession(GameSession::PREPARE),
	mCardsHandle(nullptr)
{
}

BaseGameMode::~BaseGameMode()
{
}

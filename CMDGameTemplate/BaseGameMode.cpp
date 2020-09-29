#include "BaseGameMode.h"

BaseGameMode::BaseGameMode():
	mSession(GameSession::PREPARE),
	mCardsHandle(nullptr),
	cur_player(0)
{
}

BaseGameMode::~BaseGameMode()
{
}

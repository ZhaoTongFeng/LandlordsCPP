#pragma once
#include "BaseGameMode.h"
class GameMode :
    public BaseGameMode
{
public:
    GameMode();
    ~GameMode();
    void ProcessInput()override;
    void UpdateGame(float deltaTime)override;
    void GenerateOutput(std::string& str)override;
protected:
    void HandCards()override;
    void Settle()override;
};


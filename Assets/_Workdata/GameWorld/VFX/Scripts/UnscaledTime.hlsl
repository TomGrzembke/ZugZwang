#ifndef UNSCALED_TIME_GLOBALS_INCLUDED
#define UNSCALED_TIME_GLOBALS_INCLUDED

float _UnscaledTime;
float _UnscaledDeltaTime;

void UnscaledTime_float(out float TimeUnscaled)
{
    TimeUnscaled = _UnscaledTime;
}


#endif
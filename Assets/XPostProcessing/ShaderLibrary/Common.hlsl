#ifndef COMMON_INCLUDED
#define COMMON_INCLUDED

#pragma once


// 取随机数.
float rand(float n)
{
    return frac(sin(n) * 13758.5453123 * 0.01);
}

float rand(float2 n)
{
    return frac(sin(dot(n, float2(12.9898, 78.233))) * 43758.5453);
}

#endif // COMMON_INCLUDED.
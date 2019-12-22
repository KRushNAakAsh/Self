@echo off
color 03

REM Self: Unit  Tests - SelfDrivingCar 12/21/2019

REM Real road drives - Courses 1,2,3
Self LeenaKudalkar@yahoo.com 1
if NOT %errorlevel% == 0 echo Self returned (%errorlevel%). Self Failed. Check SelfLOG*.txt
copy SelfLOG.txt SelfLOG_Course1.txt

Self ekbhakt8@gmail.com 2
if NOT %errorlevel% == 0 echo Self returned (%errorlevel%). Self Failed. Check SelfLOG*.txt
copy SelfLOG.txt SelfLOG_Course2.txt

Self LeenaKudalkar@yahoo.com 3
if NOT %errorlevel% == 0 echo Self returned (%errorlevel%). Self Failed. Check SelfLOG*.txt
copy SelfLOG.txt SelfLOG_Course3.txt

REM Now that real tests are over for all road courses, check bad command lines - should display Usage and/or throw exception.
Self qwueqo -100

Self HappyHolidays 20

Self 9345 Ho 1qc

REM Test user-friendliness with help command line switches
Self -help

Self -?

Self
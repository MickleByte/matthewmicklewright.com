import pandas as pd
import numpy as np
import matplotlib.pyplot as plt
import math

# plots scatter plot with points x and y coordinates coming from the 1d arrays x_data and y_data. Also plots the line with passed formula - colour of this line can be passed as string but is orange by default
def plotGraph(x_data, y_data, formula, colour='orange'):
    plt.plot(x_data, y_data, 'o') # plot points with coordinates in dataset x and y
    x = np.linspace(-5, 5) # graph ranges from -5 to 5 in x
    y = eval(formula) # evalauate formula so that line can be plotted
    plt.plot(x, y, color=colour) # plot line

# plots a point with coordinates x, y and a colour which is blue by default
def plotPoint(x, y, col='blue'):
    plt.plot(x, y, 'o', color=col)

# Takes vector input and sums elements together, power can be used to square or cube elements before addition but is 1 by default
def Sum(data, power=1):
    total = 0
    for item in data:
        total = total + (item ** power)
    return total

# Takes two vector inputs and calculates dot product (a1*b1+a2*b2+…), optional power can be used to square or cube x component but is 1 by default
def dotProduct(xData, yData, power=1):
    total = 0
    for x, y in zip(xData, yData):
        total = total + (y * (x ** power))
    return total

# passed the weights (coefficents of x in an array) and the max order of the polynomial, this function returns the formula for the line as a string
def genFormula(weights, order):
    formula = '' # initialise as empty string
    for i in range(order, -1, -1): # runs from the order down to -1 in steps of -1 (e.g. if order were 5, values of i would be: 5,4,3,2,1,0)
        formula = formula + str(weights[i]) + '*x**' + str(i) + '+' # append formula with current coefficent next to x to the power of i
    formula = formula[:-1] # remove last char of formula which is an additional '+' symbol
    return formula


#LHS Matrix Example:
#   for data set x = [1, 2, 3, 4, 5], and a polynomial of order 3
#   first row of LHS matrix:
#        [n=5, sum(x) = 15, sum(x^2) = 55]
#   second row: 
#        [sum(x)=15, sum(x^2) = 55, sum(x^3)]
#   third row
#        [sum(x^2), sum(x^3), sum(x^4)]


# calculates everything but the first value on the first row
def LHSFirstRow(xData, order):
    row = np.matrix([]) # initialise matrix
    for i in range(1, order + 1): # iterates i from 1 up to the order of polynomial in steps of 1
        row = np.hstack([row, np.matrix([Sum(xData, i)])]) # each value in row is sum of xData^ iterator i
    return row

# calculates one row of the matrix
def LHSRow(xData, startPower, endPower):
    row = np.matrix([])
    for i in range(startPower, endPower + 1): # iterates from a starting point to a max order for that row
        row = np.hstack([row,  np.matrix([Sum(xData, i)])]) # each value in row is sum of xData ^ iterator i
    return row


def getLHS(xData, yData, order):
    myMatrix = np.matrix([xData.size]) # first value in matrix is size of sample (number of x values)
    myMatrix = np.hstack([myMatrix, LHSFirstRow(xData, order)]) # calculates remainder of first row, first value is sum of x values, next is sum of x values squared, then cubed etc. up to x^ the order of the polynomial
    
    for i in range(1, order + 1): # iterates from 1 to order of polynomial in steps of 1
        # calculates and appends a row to matrix where starting power of x is one more than previous
        myMatrix = np.vstack([myMatrix, LHSRow(xData, i, i + order)])
    return myMatrix


def getRHS(xData, yData, order):
    myMatrix = np.array([Sum(yData)]) # initialise matrix and calculate first value as sum of y values * x^0 
    #calculate next values in matrix as dot product of y values and x values ^i, where i iterates from 1 to order of polynomial
    for i in range(order):
        myMatrix = np.hstack(
            (myMatrix, np.array([dotProduct(xData, yData, i + 1)])))
    return myMatrix


def pol_regression(features_train, y_train, degree):
    RHS = getRHS(features_train, y_train, degree) # produce the matrix on the right hand side (RHS) of equation
    LHS = getLHS(features_train, y_train, degree) # priduce matrix on left hand side (LHS) of equation
    parameters = np.linalg.solve(LHS, RHS) # solve simultaneous equations, returns matrix of x coefficients
    return parameters

# Below function calculates Root Mean Square Error (RMSE) - error = difference in y from what line predicted to actual data point
# yi = the sum of the y values as calculated by formula
# to get Mean Squared Error (MSE), do (yi - actual y values)^2
# and to get RMSE = sqrt(MSE)
#
# RMSE Example:
#   formula = 2x + 4
#   xData = [1, 2, 3, 4, 5]
#   yData = [7, 8, 8, 15, 14]
#
#   yi, when i is 1 = (2(1) + 4) = 6
#   actual y at i is 1 = 7
#   (yi - y)^2 = (6 - 7)^2 = -1^2 = 1
#
#   Repeat above for i = 1,2,3,4,5 and add together the squared differences:
#       total diff = 114
#
#   calculate the MSE: 
#       114 / number of data items (5) = 22.8
#   and sqrt to find final RMSE: 
#       sqrt(22.8) = 4.77
def eval_pol_regression(weights, xData, yData, degree):
    mse = 0 # initilase mean square error as 0
    for x, y in zip(xData, yData): # for each item in data sets x and y
        yi = 0
        for i in range(degree, -1, -1): # runs from the order down to -1 in steps of -1 (e.g. if order were 5, values of i would be: 5,4,3,2,1,0)
            yi = yi + (weights[i] * (x ** i)) # sum up the weight * x^i to find the value of a given yi
        mse = mse + ((yi - y) ** 2) # get the difference between lines prediction (yi) and the actual data point (y), and square it to get squared error
    mse = mse / len(xData) # calcualte mean squared error by dividing total squared error by number of data points
    rmse = math.sqrt(mse) # find RMSE from MSE by square rooting
    return rmse

# Runs code for polynomial regression task
def Task1_2():
    #import data from csv file
    dataset = pd.read_csv('Task1Data.csv')
    #seperate out into x and y data
    x_data = dataset['x'].as_matrix()
    y_data = dataset['y'].as_matrix()
    order = int(input('\nEnter degree of polynomial (e.g 0, 2, 3, etc.):     '))
    #performs regression to find x coefficients and returns them as array of weights
    weights = pol_regression(x_data, y_data, order)
    #goes through weights (coefficients of x) and appends them to a string 'formula' so that it can be plotted
    formula = ''
    for i in range(order, -1, -1): # runs from the order down to -1 in steps of -1 (e.g. if order were 5, values of i would be: 5,4,3,2,1,0)
        formula = formula + str(weights[i]) + '*x**' + str(i) + '+' # appends with the coefficent, the x and the power of the x as well as + to connect it to the next term
    formula = formula[:-1] # remove last '+' symbol from string
    plotGraph(x_data, y_data, formula)
    print('Line has equation: ', formula) # prints out formula for user so they can see exact weights/ x coefficients
    print('RMSE = ', eval_pol_regression(weights, x_data, y_data, order)) # prints RMSE for that line

# Runs code for evaluation task
def Task1_3():
    fig = plt.figure()
    dataset = pd.read_csv('Task1Data.csv') # read data from csv file
    #split data into x and y
    x_data = dataset['x'].as_matrix()
    y_data = dataset['y'].as_matrix()

    # height of data = number of data items - needed to work out num items to create 70:30 split between training and test sets
    height = len(x_data)
    # training data is first 70% of total dataset
    x_dataTrain = x_data[0:math.floor(height * 0.7)]
    y_dataTrain = y_data[0:math.floor(height * 0.7)]

    # Testing data is last 30% of total dataset
    x_dataTest = x_data[math.ceil(height * 0.3): height]
    y_dataTest = y_data[math.ceil(height * 0.3): height]

    RMSEsTrain = []
    RMSEsTest = []

    # loop just runs from 0 to 10 as we want to test polynomials with degrees 0 to 5 as well as 10
    for i in range(0, 11):
        #Run polynomial regression with training data
        weight = pol_regression(x_dataTrain, y_dataTrain, i)

        #calculate the RMSE for the training data
        rmse = eval_pol_regression(weight, x_dataTrain, y_dataTrain, i)
        #plot RMSE in blue
        plotPoint(i, rmse, 'blue')
        RMSEsTrain.append(rmse)
        # calculate RMSE for test data
        rmse = eval_pol_regression(weight, x_dataTest, y_dataTest, i)
        # plot RMSE in red
        plotPoint(i, rmse, 'red')
        RMSEsTest.append(rmse)

    #print out RMSE values as a table
    print('Degree      Train       Test')
    for i in range(0,11):
        print(i, '   ', RMSEsTrain[i], '   ', RMSEsTest[i])

    # Put labels on axes & print label to id training and test data by data point colour
    ax1 = fig.add_subplot()
    ax1.set_ylabel('RMSE')
    ax2 = fig.add_subplot()
    ax2.set_xlabel('Degree of Polynomial')
    print('\nRed points are the test set, blue are training set')

# Main code - takes input 1 or 2 depending if user wants functionality for task 1.2 or task 1.3
choice = int(input("Enter number of choice:\n1. Task 1.2 Implementation of Polynomial Regression\n2. Task 1.3 Evaluation of Regression Code\n"))
if choice == 1:
    Task1_2()
else:
    Task1_3()

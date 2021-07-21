# Matt Micklewright Feb 2020
# Program searches for the first occurance of a number n in the digits of pi, 
# once occurance is found at position i the program is run again with n = i until a full loop back to the orignal n is made or until index of n = n
# see this Numberphile video for a more detailed explanation: https://www.youtube.com/watch?v=W20aT14t8Pw



# digits of pi generation based on spigot algorithm, explained here http://www.cs.ox.ac.uk/people/jeremy.gibbons/publications/spigot.pdf
def calcPi(limit, terms = [1, 0, 1, 1, 3, 3]):
    returnVal = []
    counter = 0 # tracks how many digits have been generated

    # while we've generated less digits than the limit
    while (counter < limit):
        if 4 *  terms[0] + terms[1] - terms[2] < terms[4] * terms[2]:
                returnVal.append(terms[4]) # return digit
                counter += 1 # increment counter
                nr = 10 * (terms[1] - terms[4] * terms[2])
                terms[4] = ((10 * (3 *  terms[0] + terms[1])) // terms[2]) - 10 * terms[4]
                terms[0] *= 10
                terms[1] = nr
        else:
                nr = (2 *  terms[0] + terms[1]) * terms[5]
                nn = ( terms[0] * (7 * terms[3]) + 2 + (terms[1] * terms[5])) // (terms[2] * terms[5])
                terms[0] *= terms[3]
                terms[2] *= terms[5]
                terms[5] += 2
                terms[3] += 1
                terms[4] = nn
                terms[1] = nr
    return returnVal, terms

def getPosistionOf(num, pi):
    # go through every digit of pi
    for i in range(0, len(pi)):  
        # we need to check digit i as well as len(num) digits to the right of i, to see if they match num
        #  e.g. if num = 159, we would check i = 0 up to i = 2, because the length of num is 3
        if (int(''.join(map(str,pi[i: i + len(str(num))]))) == num):
            # if we find a match, we can return the index of the match, i
            return i

def findLoop(num, exitNum, pi, digitLength, genTerms, steps = 0):
    pos = getPosistionOf(num, pi) # for the num, find the index of its first appearance in pi
    # if the number could not be found in the currently known digits of pi, it will return none
    if (pos == None):
        digitLength = digitLength * 2 # double the digit length
        newPiDigits, genTerms = calcPi(digitLength, genTerms) # and re-gen pi with this double digit limit
        # append all of the new digits onto the end of the pi array
        for d in newPiDigits:
            pi.append(d)
        return findLoop(num, exitNum, pi, digitLength, genTerms, steps) # try to find the numbers position again with the doubled search space
    else:
        # if the number was found:
        if (pos == num): # if the position is the same as the number, it is self IDing therefore the loop terminates here
            print(pos, "Is Self Identifying")
            return steps
        steps += 1
        if (pos == exitNum): # if the position is the same as the exit case then we have completed a full loop and can stop searching for the next number
            print(pos, "\nLoop is Closed")
            return steps
        else:
            # otherwise, output the position
            print(pos)
            # and call this function again with the position as our new search number
            return findLoop(pos, exitNum, pi, digitLength, genTerms, steps)

digitLength = 1000 # initialise number of pi digits to generate as 1000, this will be increased as needed
userInput = int(input("Enter Integer: ")) # get user's input
pi, genTerms = calcPi(digitLength) # generate (digitLength = 1000) pi digits, also save the terms as genTerms so calculation of pi can be restarted later if nessecary
pi = pi[1:] # don't want the first digit (3) as it is not a decimal & we want to index our decimal places starting at 1
steps = findLoop(userInput, userInput, pi, digitLength, genTerms) # calculate the number of steps required to complete the loop of the user's number
print("Required", steps, "steps to resolve") # output number of steps
input("Press Any Key To Exit...")
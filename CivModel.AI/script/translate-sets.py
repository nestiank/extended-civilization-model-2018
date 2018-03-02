import sys, os
import re

data = []

with open(r'input.txt', encoding='utf-8') as input:
    for line in input:
        line = line.strip()
        if line == '' or line == '---': continue

        idx = line.find(':')
        if idx is -1: continue
        name = line[:idx]
        line = line[idx + 1:]

        idx = line.find('[')
        if idx is -1:
            idx = line.find('<')
        if idx is -1: continue
        line = line[idx + 1:].lstrip()

        lst = []
        while line is not '':
            idx = line.find(',')
            if idx is -1:
                idx = line.find(']')
                if idx is -1:
                    idx = line.find('>')
                if idx is -1: break
            lst.append(float(line[:idx]))
            line = line[idx + 1:].lstrip()

        data.append((name, lst))

result = []
prefix = ''
idx = 0
while idx < len(data):
    if data[idx][0][:len(prefix)] != prefix:
        prefix = ''
    if prefix == '':
        if idx + 1 < len(data):
            idx2 = 0
            while idx2 < min(len(data[idx][0]), len(data[idx + 1][0])):
                if data[idx][0][idx2] != data[idx + 1][0][idx2]:
                    break
                idx2 = idx2 + 1
            if idx2 > 0:
                prefix = data[idx][0][:idx2]
    result.append((prefix if prefix != '' else data[idx][0], data[idx][0], data[idx][1]))
    idx = idx + 1

with open(r'output.txt', 'w', encoding='utf-8') as output:
    nowfix = None
    for (prefix, name, lst) in result:
        if prefix != nowfix:
            if nowfix != None:
                output.write('        ]\n')
            nowfix = prefix
            output.write('    let %sSets =\n' % prefix)
            output.write('        fuzzy.CreateSetsByList [\n')
        output.write('            "%s", [ ' % name)
        flag = False
        for val in lst:
            if flag: output.write('; ')
            if val == float('inf'):
                output.write('infinityf')
            elif val == float('-inf'):
                output.write('-infinityf')
            else:
                output.write('%.3ff' % val)
            flag = True
        output.write(' ];\n')
    output.write('        ]\n')

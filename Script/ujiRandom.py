from numpy import random
import matplotlib.pyplot as plt
import pandas as pd

def next_double(min: float, max: float) -> float:
    r = random.rand()
    return r * (max - min) + min

def graph_random(func, count: int = 100000):
    df = pd.DataFrame({'random':[func() for x in range(0, count)]})
    df.hist('random')
    plt.show(block=True)

graph_random(lambda: next_double(-5.12, 5.12))
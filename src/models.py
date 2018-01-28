'''
Models
Define the different NN models we will use
Author: Tawn Kramer
'''
from __future__ import print_function
from keras.models import Sequential
from keras.layers import Conv2D, MaxPooling2D
from keras.layers import Dense, Lambda, ELU
from keras.layers import Activation, Dropout, Flatten, Dense
from keras.layers import Cropping2D
from keras.optimizers import Adadelta

import conf

def show_model_summary(model):
    model.summary()
    for layer in model.layers:
        print(layer.output_shape)

def get_nvidia_model(num_outputs, fine_tuning=False, base_model=None):
    '''
    this model is inspired by the NVIDIA paper
    https://images.nvidia.com/content/tegra/automotive/images/2016/solutions/pdf/end-to-end-dl-using-px.pdf
    Activation is ELU
    '''
    print('Creating NVIDIA model, fine tune is %s' % str(fine_tuning))
    row, col, ch = conf.row, conf.col, conf.ch
    trainable = not fine_tuning
    model = Sequential()

    model.add(Cropping2D(cropping=((60,0), (0,0)), input_shape=(row, col, ch)))

    model.add(Lambda(lambda x: x/127.5 - 1.))
    model.add(Conv2D(24, (5, 5), strides=(2, 2), padding="same", trainable=trainable))
    model.add(ELU())
    model.add(Conv2D(32, (5, 5), strides=(2, 2), padding="same", trainable=trainable))
    model.add(ELU())
    model.add(Conv2D(64, (5, 5), strides=(2, 2), padding="same", trainable=trainable))
    model.add(ELU())
    model.add(Conv2D(64, (3, 3), strides=(2, 2), padding="same", trainable=trainable))
    model.add(ELU())
    model.add(Dropout(.5))
    model.add(Conv2D(64, (3, 3), strides=(1, 1), padding="same", trainable=trainable))
    model.add(Flatten())
    model.add(Dense(1000, trainable=trainable))
    model.add(Dropout(.1))
    model.add(ELU())
    model.add(Dense(128, trainable=trainable))
    model.add(ELU())
    model.add(Dense(num_outputs))

    if fine_tuning:
        model.load_weights(base_model)

    model.compile(optimizer=Adadelta(), loss="mse")
    return model

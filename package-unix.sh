#!/bin/bash

# get arguments and shift
PACKAGE_DIR="$1"
CONFIGURATION="$2"
PLATFORM="$3"
ARCHITECTURE="$4"
OUTPUT_FILE="$5"

# set other variables
LIBRARY_DIR="$PACKAGE_DIR/lib"
INCLUDE_DIR="$PACKAGE_DIR/include"
BINARY_DIR="bin/$CONFIGURATION-$PLATFORM-$ARCHITECTURE"
COPY_COMMAND="cp -rf"

# create directories
mkdir $PACKAGE_DIR
mkdir $LIBRARY_DIR
mkdir $INCLUDE_DIR

# copy external data to package directory
echo "Copying external data..."
$COPY_COMMAND entrypoint/data $PACKAGE_DIR
$COPY_COMMAND entrypoint/mono $PACKAGE_DIR
$COPY_COMMAND entrypoint/script-assemblies $PACKAGE_DIR

# copy binaries to package directory
echo "Copying binaries..."
$COPY_COMMAND $BINARY_DIR/engine/*engine* $LIBRARY_DIR
$COPY_COMMAND $BINARY_DIR/entrypoint/entrypoint* $PACKAGE_DIR

# copy headers to package directory
echo "Copying headers..."
$COPY_COMMAND engine/include $INCLUDE_DIR
rm -rf $INCLUDE_DIR/fe_engine
mv $INCLUDE_DIR/include $INCLUDE_DIR/fe_engine

# package it into a tarball
echo "Compressing..."
tar -cf $OUTPUT_FILE $PACKAGE_DIR

# delete intermediate directory
echo "Deleting intermediate directory..."
rm -rf $PACKAGE_DIR

echo "Done! Output written to $OUTPUT_FILE"
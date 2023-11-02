import React, { useEffect } from 'react';
import { useAppDispatch, useAppSelector } from '../storeHooks';
import { getPhotos, selectPhotos } from './photosSlice';
import { Box, ImageList, ImageListItem, ListSubheader } from '@mui/material';

// TODO: Add thumbnail width and height to the media data model
const largestSize = 224;
const smallestSize = 168;

function HomePage() {
  const photos = useAppSelector(selectPhotos);
  const dispatch = useAppDispatch();

  useEffect(() => {
    dispatch(getPhotos());
  }, []);

  const year = photos.length > 0 ? new Date(photos[0].dateTimeOriginal * 1000).getFullYear() : 1980;

  return (
    <>
      <p>Hello there</p>
      <Box
        sx={{
          display: 'flex',
          flexWrap: 'wrap',
          gap: '10px',
          rowGap: '10px',
          justifyContent: 'space-evenly',
        }}
      >
        {photos.length > 0 &&
          photos.map((photo) => (
            <img
              key={photo.id}
              src={`${photo.thumbnailUrl}?w=${
                photo.width > photo.height ? largestSize : smallestSize
              }&h=${photo.width < photo.height ? largestSize : smallestSize}`}
              alt={photo.fileName}
              loading="lazy"
            />
          ))}
      </Box>
      {/*<ImageList cols={4}>*/}
      {/*  <ImageListItem key="Subheader" cols={4}>*/}
      {/*    <ListSubheader component="div">{year}</ListSubheader>*/}
      {/*  </ImageListItem>*/}
      {/*  {photos.length > 0 &&*/}
      {/*    photos.map((photo) => (*/}
      {/*      <ImageListItem key={photo.id}>*/}
      {/*        <img*/}
      {/*          srcSet={`${photo.thumbnailUrl}?w=${*/}
      {/*            photo.width > photo.height ? largestSize : smallestSize*/}
      {/*          }&fit=crop&auto=format&dpr=2 2x`}*/}
      {/*          src={`${photo.thumbnailUrl}?w=${*/}
      {/*            photo.width > photo.height ? largestSize : smallestSize*/}
      {/*          }&fit=crop&auto=format`}*/}
      {/*          alt={photo.fileName}*/}
      {/*          loading="lazy"*/}
      {/*        />*/}
      {/*      </ImageListItem>*/}
      {/*    ))}*/}
      {/*</ImageList>*/}
    </>
  );
}

export default HomePage;

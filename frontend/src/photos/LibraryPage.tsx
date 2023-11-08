import React, { useEffect } from 'react';
import { useAppDispatch, useAppSelector } from '../storeHooks';
import { getPhotos, selectPhotos } from './photosSlice';
import { Box } from '@mui/material';
import { MediaCard } from './MediaCard';
import { ScrollableBox } from '../components/ScrollableBox';

const topBarHeight = 56;

function LibraryPage() {
  // TODO: Add nullable DateOfLastPhoto to the photos store
  const photos = useAppSelector(selectPhotos);
  const dispatch = useAppDispatch();

  useEffect(() => {
    // TODO: Pass nullable DateOfLastPhoto to getPhotos to filter
    dispatch(getPhotos());
  }, []);

  const handleScrollToTop = (): void => {
    // TODO: Introduce new thunk - getPreviousChunk, and pass there DateOfLastPhoto
    console.log('Scroll to top!!');
  };

  const handleScrollToBottom = (): void => {
    // TODO: Introduce new thunk - getNextChunk, and pass there DateOfLastPhoto
    console.log('Scroll to bottom!!');
  };

  return (
    <>
      <Box>
        <p> Hello there, {topBarHeight} </p>
      </Box>
      <ScrollableBox
        indent={topBarHeight}
        scrollToTop={handleScrollToTop}
        scrollToBottom={handleScrollToBottom}
      >
        <Box
          sx={{
            display: 'flex',
            flexWrap: 'wrap',
            gap: '10px',
            rowGap: '10px',
            justifyContent: 'space-evenly',
            alignItems: 'center',
          }}
        >
          {photos.length > 0 &&
            photos.map((photo) => <MediaCard key={`media-card-id-${photo.id}`} media={photo} />)}
        </Box>
      </ScrollableBox>
    </>
  );
}

export default LibraryPage;

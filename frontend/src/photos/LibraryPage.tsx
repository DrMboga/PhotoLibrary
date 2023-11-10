import React, { useEffect, useState } from 'react';
import { useAppDispatch, useAppSelector } from '../storeHooks';
import {
  getNextPhotosChunk,
  getPhotos,
  getPreviousPhotosChunk,
  selectDateOfFirstPhoto,
  selectDateOfLastPhoto,
  selectPhotos,
  selectPhotosLoadingBottom,
  selectPhotosLoadingTop,
} from './photosSlice';
import { Box } from '@mui/material';
import { MediaCard } from './MediaCard';
import { ScrollableBox } from '../components/ScrollableBox';
import { dateFromUnixTime } from '../helpers/date-helper';
import CircularProgress from '@mui/material/CircularProgress';
import { MediaPreview } from './MediaPreview';

const topBarHeight = 56;

function LibraryPage() {
  const loadingTop = useAppSelector(selectPhotosLoadingTop);
  const loadingBottom = useAppSelector(selectPhotosLoadingBottom);
  const dateOfFirstPhoto = useAppSelector(selectDateOfFirstPhoto);
  const dateOfLastPhoto = useAppSelector(selectDateOfLastPhoto);
  const photos = useAppSelector(selectPhotos);
  const dispatch = useAppDispatch();

  const [selectedMediaId, setSelectedMediaId] = useState('');

  useEffect(() => {
    dispatch(getPhotos(dateOfFirstPhoto));
  }, [dispatch]);

  const handleScrollToTop = (): void => {
    if (dateOfFirstPhoto) {
      setSelectedMediaId('');
      dispatch(getPreviousPhotosChunk(dateOfFirstPhoto));
    }
  };

  const handleScrollToBottom = (): void => {
    if (dateOfLastPhoto) {
      setSelectedMediaId('');
      dispatch(getNextPhotosChunk(dateOfLastPhoto));
    }
  };

  const handleCardClick = (mediaId: string) => {
    setSelectedMediaId(mediaId);
  };

  return (
    <>
      <Box>
        <p>
          {dateOfFirstPhoto && dateOfLastPhoto
            ? `${dateFromUnixTime(dateOfFirstPhoto).toLocaleString('ru-RU')} - ${dateFromUnixTime(
                dateOfLastPhoto,
              ).toLocaleString('ru-RU')}`
            : ''}
        </p>
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
          {loadingTop && <CircularProgress />}
          {photos.length > 0 &&
            photos.map((photo) =>
              photo.id === selectedMediaId ? (
                <MediaPreview media={photo} key={`media-preview-card-id${photo.id}`}></MediaPreview>
              ) : (
                <MediaCard
                  key={`media-card-id-${photo.id}`}
                  media={photo}
                  onClick={handleCardClick}
                />
              ),
            )}
          {loadingBottom && <CircularProgress />}
        </Box>
      </ScrollableBox>
    </>
  );
}

export default LibraryPage;

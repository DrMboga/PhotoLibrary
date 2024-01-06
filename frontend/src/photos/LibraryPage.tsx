import React, { useState } from 'react';
import { useAppSelector } from '../storeHooks';
import {
  selectDateOfFirstPhoto,
  selectDateOfLastPhoto,
  selectPhotosLibraryError,
  selectPhotosLoadingBottom,
  selectPhotosLoadingTop,
} from './photosSlice';
import { Alert, Box, IconButton } from '@mui/material';
import { MediaCard } from './MediaCard';
import { ScrollableBox } from '../components/ScrollableBox';
import { dateFromUnixTime } from '../helpers/date-helper';
import CircularProgress from '@mui/material/CircularProgress';
import { MediaPreview } from './MediaPreview';
import { useMediaSignalRHub } from './useMediaSignalRHub';
import ArrowCircleLeftIcon from '@mui/icons-material/ArrowCircleLeft';
import ArrowCircleRightIcon from '@mui/icons-material/ArrowCircleRight';
import { MediaInfo } from '../model/media-info';

const topBarHeight = 56;

function LibraryPage() {
  const loadingTop = useAppSelector(selectPhotosLoadingTop);
  const loadingBottom = useAppSelector(selectPhotosLoadingBottom);
  const dateOfFirstPhoto = useAppSelector(selectDateOfFirstPhoto);
  const dateOfLastPhoto = useAppSelector(selectDateOfLastPhoto);
  const error = useAppSelector(selectPhotosLibraryError);

  const [selectedMedia, setSelectedMedia] = useState<MediaInfo>();
  const [dialogOpen, setDialogOpen] = React.useState(false);

  const { connection, getNextPhotosChunkFromBackend, getPreviousPhotosChunkFromBackend, photos } =
    useMediaSignalRHub(dateOfFirstPhoto);

  const handleScrollToTop = (): void => {
    setSelectedMedia(undefined);
    if (dateOfFirstPhoto) {
      if (connection) {
        getPreviousPhotosChunkFromBackend(dateOfFirstPhoto, connection).catch((err) =>
          console.error(err),
        );
      }
    }
  };

  const handleScrollToBottom = (): void => {
    setSelectedMedia(undefined);
    if (dateOfLastPhoto) {
      if (connection) {
        getNextPhotosChunkFromBackend(dateOfLastPhoto, connection).catch((err) =>
          console.error(err),
        );
      }
    }
  };

  const handleCardClick = (media: MediaInfo) => {
    setSelectedMedia(media);
    setDialogOpen(true);
  };

  const handleDialogClose = () => {
    setDialogOpen(false);
    setSelectedMedia(undefined);
  };

  return (
    <>
      {error && <Alert severity="error">{error}</Alert>}
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
          <IconButton color="primary" aria-label="more..." onClick={handleScrollToTop}>
            <ArrowCircleLeftIcon />
          </IconButton>
          {photos.length > 0 &&
            photos.map((photo) => (
              <MediaCard
                key={`media-card-id-${photo.id}`}
                media={photo}
                onClick={handleCardClick}
              />
            ))}
          <IconButton color="primary" aria-label="more..." onClick={handleScrollToBottom}>
            <ArrowCircleRightIcon />
          </IconButton>
          {loadingBottom && <CircularProgress />}
        </Box>
      </ScrollableBox>
      <MediaPreview media={selectedMedia} open={dialogOpen} onClose={handleDialogClose} />
    </>
  );
}

export default LibraryPage;

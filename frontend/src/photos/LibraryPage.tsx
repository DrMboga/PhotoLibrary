import React, { useState } from 'react';
import { useAppDispatch, useAppSelector } from '../storeHooks';
import {
  selectDateOfFirstPhoto,
  selectDateOfLastPhoto,
  selectPhotosLibraryError,
  setLoadingBottom,
  setLoadingTop,
} from './photosSlice';
import { Alert, Box, IconButton } from '@mui/material';
import { MediaCard } from './MediaCard';
import { ScrollableBox } from '../components/ScrollableBox';
import { currentDateLinuxTime, dateFromUnixTime } from '../helpers/date-helper';
import CircularProgress from '@mui/material/CircularProgress';
import { MediaPreview } from './MediaPreview';
import { useMediaSignalRHub } from './useMediaSignalRHub';
import ArrowCircleLeftIcon from '@mui/icons-material/ArrowCircleLeft';
import ArrowCircleRightIcon from '@mui/icons-material/ArrowCircleRight';
import { MediaInfo } from '../model/media-info';
import Drawer from '@mui/material/Drawer';
import EditCalendarIcon from '@mui/icons-material/EditCalendar';
import { DatesFilterComponent } from './DatesFilterComponent';

const topBarHeight = 56;
const oneYear = 31536000;

function LibraryPage() {
  const dispatch = useAppDispatch();
  const dateOfFirstPhoto = useAppSelector(selectDateOfFirstPhoto);
  const dateOfLastPhoto = useAppSelector(selectDateOfLastPhoto);
  const error = useAppSelector(selectPhotosLibraryError);

  const [selectedMedia, setSelectedMedia] = useState<MediaInfo>();
  const [dialogOpen, setDialogOpen] = useState(false);
  const [drawerOpen, setDrawerOpen] = useState(false);

  const {
    connection,
    getNextPhotosChunkFromBackend,
    getPreviousPhotosChunkFromBackend,
    photos,
    cleanPhotos,
    handleDeleteCard,
    handleAlbumMarkChanged,
    loadingTop,
    loadingBottom,
  } = useMediaSignalRHub(dateOfFirstPhoto);

  const handleScrollToTop = (): void => {
    setSelectedMedia(undefined);
    if (dateOfFirstPhoto) {
      if (connection && !loadingTop) {
        dispatch(setLoadingTop());
        getPreviousPhotosChunkFromBackend(dateOfFirstPhoto, connection).catch((err) =>
          console.error(err),
        );
      }
    }
  };

  const handleScrollToBottom = (): void => {
    setSelectedMedia(undefined);

    const dateFrom = dateOfLastPhoto ?? currentDateLinuxTime() - oneYear;

    console.log('dateOfLastPhoto', dateFrom, connection?.state);
    if (connection && !loadingBottom) {
      dispatch(setLoadingBottom());
      getNextPhotosChunkFromBackend(dateFrom, connection).catch((err) => console.error(err));
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

  const toggleDrawer = (open: boolean) => (event: React.KeyboardEvent | React.MouseEvent) => {
    if (
      event.type === 'keydown' &&
      ((event as React.KeyboardEvent).key === 'Tab' ||
        (event as React.KeyboardEvent).key === 'Shift')
    ) {
      return;
    }

    setDrawerOpen(open);
  };

  const newDateSelectedHandle = (newDate: number) => {
    if (connection) {
      cleanPhotos();
      getNextPhotosChunkFromBackend(newDate, connection).catch((err) => console.error(err));
    }
  };

  return (
    <>
      {error && <Alert severity="error">{error}</Alert>}
      <Drawer anchor="top" open={drawerOpen} onClose={toggleDrawer(false)}>
        <DatesFilterComponent
          currentDate={dateOfFirstPhoto}
          newDateSelected={newDateSelectedHandle}
        />
      </Drawer>
      <Box sx={{ display: 'flex', gap: '5px' }}>
        <p>
          {dateOfFirstPhoto && dateOfLastPhoto
            ? `${dateFromUnixTime(dateOfFirstPhoto).toLocaleString('ru-RU')} - ${dateFromUnixTime(
                dateOfLastPhoto,
              ).toLocaleString('ru-RU')}`
            : ''}
        </p>
        <IconButton color="secondary" aria-label="Select date" onClick={toggleDrawer(true)}>
          <EditCalendarIcon fontSize="small" />
        </IconButton>
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
      <MediaPreview
        media={selectedMedia}
        open={dialogOpen}
        onClose={handleDialogClose}
        deleteCardFromList={handleDeleteCard}
        albumMarkChanged={handleAlbumMarkChanged}
      />
    </>
  );
}

export default LibraryPage;

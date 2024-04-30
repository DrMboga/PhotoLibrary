import React, { useEffect, useState } from 'react';
import { useAppDispatch, useAppSelector } from '../storeHooks';
import {
  selectDateOfFirstPhoto,
  selectDateOfLastPhoto,
  selectPhotosLibraryError,
  setLoadingBottom,
  setLoadingTop,
} from './photosSlice';
import { Alert, Badge, Box, FormControlLabel, IconButton, Switch } from '@mui/material';
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
import DeleteIcon from '@mui/icons-material/Delete';
import { backendAPI } from '../api/BackendApi';

const topBarHeight = 56;
const oneYear = 31536000;
const threeSeconds = 3;

function LibraryPage() {
  const dispatch = useAppDispatch();
  const dateOfFirstPhoto = useAppSelector(selectDateOfFirstPhoto);
  const dateOfLastPhoto = useAppSelector(selectDateOfLastPhoto);
  const error = useAppSelector(selectPhotosLibraryError);

  const [selectedMedia, setSelectedMedia] = useState<MediaInfo>();
  const [dialogOpen, setDialogOpen] = useState(false);
  const [drawerOpen, setDrawerOpen] = useState(false);
  const [lastRequestTime, setLastRequestTime] = useState(currentDateLinuxTime());
  const [selectMultipleMedia, setSelectMultipleMedia] = useState<boolean>(false);
  const [selectedMediaIds, setSelectedMediaIds] = useState<string[]>([]);
  const [triggeredDeleteMultiple, setTriggeredDeleteMultiple] = useState<boolean>(false);

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
    handleDeleteSeveralCards,
    authToken,
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

    const now = currentDateLinuxTime();
    const debounce = now - lastRequestTime < threeSeconds;
    console.log('debounce', now - lastRequestTime, debounce);

    if (connection && !loadingBottom && !debounce) {
      setLastRequestTime(now);
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

  const handleCardCheck = (mediaId: string) => {
    if (selectedMediaIds.includes(mediaId)) {
      const copy = [...selectedMediaIds];
      const index = copy.indexOf(mediaId);
      copy.splice(index, 1);
      setSelectedMediaIds(copy);
    } else {
      setSelectedMediaIds([...selectedMediaIds, mediaId]);
    }
  };

  useEffect(() => {
    if (selectedMediaIds.length === 0) {
      return;
    }
    if (!selectMultipleMedia) {
      setSelectedMediaIds([]);
    }
  }, [selectMultipleMedia, selectedMediaIds]);

  useEffect(() => {
    if (!triggeredDeleteMultiple || selectedMediaIds.length === 0) {
      return;
    }
    backendAPI.deleteBunchOfMedias(selectedMediaIds, authToken).then(() => {
      setTriggeredDeleteMultiple(false);
      handleDeleteSeveralCards(selectedMediaIds);
      setSelectedMediaIds([]);
    });
  }, [triggeredDeleteMultiple, selectedMediaIds, authToken]);

  return (
    <>
      {error && <Alert severity="error">{error}</Alert>}
      <Drawer anchor="top" open={drawerOpen} onClose={toggleDrawer(false)}>
        <DatesFilterComponent
          currentDate={dateOfFirstPhoto}
          newDateSelected={newDateSelectedHandle}
        />
      </Drawer>
      <Box sx={{ display: 'flex', justifyContent: 'space-between' }}>
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
        <Box sx={{ display: 'flex', gap: '5px' }}>
          <FormControlLabel
            control={
              <Switch
                value={selectMultipleMedia}
                onClick={() => setSelectMultipleMedia(!selectMultipleMedia)}
              />
            }
            label="Select multiple"
          />
          {selectedMediaIds.length > 0 && (
            <IconButton aria-label="delete" onClick={() => setTriggeredDeleteMultiple(true)}>
              <Badge badgeContent={selectedMediaIds.length} color="secondary">
                <DeleteIcon />
              </Badge>
            </IconButton>
          )}
        </Box>
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
                showSelect={selectMultipleMedia}
                onSelect={handleCardCheck}
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
